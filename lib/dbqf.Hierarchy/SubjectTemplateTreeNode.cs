﻿using System;
using System.Collections.Generic;
using System.Text;
using dbqf.Configuration;
using System.Diagnostics;
using dbqf.Criterion;
using System.Collections;
using dbqf.Hierarchy.Display;
using dbqf.Sql;
using System.Text.RegularExpressions;
using dbqf.Hierarchy.Data;
using System.Data;

namespace dbqf.Hierarchy
{
    /// <summary>
    /// Represents a template from which to retrieve the actual data nodes of a tree hierarchy.
    /// </summary>
    [DebuggerDisplay("{Subject}: {Text}")]
    public class SubjectTemplateTreeNode : TemplateTreeNode
    {
        protected static readonly Regex PLACEHOLDER_RE = new Regex(@"\{[^\}]+\}");

        public SubjectTemplateTreeNode(IDataSource source)
            : base()
        {
            _source = source;
        }
        readonly IDataSource _source;

        /// <summary>
        /// Occurs prior to loading nodes for this template. Parameters can be modified or the request cancelled by the handler.
        /// This event also bubbles from it's children so it can be handled by registering with the root node.
        /// </summary>
        public event EventHandler<Events.DataSourceLoadEventArgs> DataSourceLoad;

        /// <summary>
        /// The subject of this node.  This defines what type of item to load at this level of the tree.
        /// </summary>
        public ISubject Subject
        {
            get { return _subject; }
            set 
            { 
                _subject = value;
                //SubjectID = (_subject != null ? _subject.ID : 0);
            }
        }
        private ISubject _subject;

        /// <summary>
        /// Gets or sets the template node text that can include tags. e.g. "Product {Number}: {Name}" -> "Product 123: Mountain Bike"
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        
        /// <summary>
        /// Defines how many levels up the tree this node needs to traverse to gather search parameters to narrow results at this level.
        /// For example, 0 means don't gather parameters at all; 1 means get parameters from the level above this one; 3 means gather parameters from
        /// the level above this one, followed by the level above that, and above that.
        /// </summary>
        public int SearchParameterLevels
        {
            get { return _searchParamLevels; }
            set { _searchParamLevels = value; }
        }
        private int _searchParamLevels;

        /// <summary>
        /// Compiles target, fields and where criteria, firing the DataSourceLoad event and returns the final args for execution with IDataSource.GetData().
        /// </summary>
        public virtual Events.DataSourceLoadEventArgs PrepareQuery(DataTreeNodeViewModel parent)
        {
            var fields = new List<IFieldPath>(GetPlaceholders(Text).Values);
            if (fields.Find(p => p.Last.Equals(Subject.IdField)) == null)
                fields.Insert(0, FieldPath.FromDefault(Subject.IdField));

            var where = new dbqf.Sql.Criterion.SqlConjunction();
            var curParent = parent;
            for (int i = 0; i < SearchParameterLevels && curParent != null; i++)
            {
                var template = curParent.TemplateNode as SubjectTemplateTreeNode;
                if (template != null)
                {
                    where.Add(new dbqf.Sql.Criterion.SqlSimpleParameter(
                        template.Subject.IdField, "=",
                        curParent.Data[template.Subject.IdField.SourceName]));
                }

                if (curParent.TemplateNode.Parameters != null)
                    where.Add(curParent.TemplateNode.Parameters);

                curParent = curParent.Parent as DataTreeNodeViewModel;
            }

            if (where.Count == 0)
                where = null;

            // allow interception of what we'll be requesting from the data source
            var args = new Events.DataSourceLoadEventArgs(Subject, fields, (where != null && where.Count == 1 ? where[0] : where));
            DataSourceLoad?.Invoke(this, args);
            return args;
        }

        /// <summary>
        /// Load nodes for this template node.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<DataTreeNodeViewModel> Load(DataTreeNodeViewModel parent)
        {
            var args = PrepareQuery(parent);
            if (args.Cancel)
                yield break;
            var data = _source.GetData(args.Target, args.Fields, args.Where);

            // precompile keys from field paths in columns
            var keys = new List<string>();
            foreach (DataColumn col in data.Columns)
            { 
                // result.Columns[i].ExtendedProperties["FieldPath"] = IFieldPath
                keys.Add(GetFieldPathPlaceholder((IFieldPath)col.ExtendedProperties["FieldPath"]));
            }

            foreach (DataRow row in data.Rows)
            {
                var node = new DataTreeNodeViewModel(this, parent, Children.Count > 0);

                // add data from the datasource to the collection
                for (int i = 0; i < data.Columns.Count; i++)
                    node.Data.Add(keys[i], row[i]);

                // add any provided data from an observer to each node too
                foreach (var pair in args.Data)
                    node.Data.Add(pair.Key, pair.Value);

                node.Text = ReplacePlaceholders(Text, node.Data);
                yield return node;
            }
        }

        /// <summary>
        /// Given an IFieldPath, create a placeholder string.
        /// e.g. IFieldPath[ "RelationField1", "Field2" ] returns "RelationField1.Field2"
        /// </summary>
        protected virtual string GetFieldPathPlaceholder(IFieldPath path)
        {
            var sb = new StringBuilder();
            foreach (var part in path)
                sb.Append($"{part.SourceName}.");

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : null;
        }

        /// <summary>
        /// Given a placeholder string, extract the field paths from this template's subject.
        /// e.g. "A string with {RelationField1.Field2}" returns { "RelationField1.Field2": IFieldPath[ "RelationField1", "Field2" ] }
        /// </summary>
        protected virtual Dictionary<string, IFieldPath> GetPlaceholders(string placeholderText)
        {
            var paths = new Dictionary<string, IFieldPath>();

            var matches = PLACEHOLDER_RE.Matches(placeholderText);
            foreach (Match m in matches)
            {
                // {field}
                // {relationfield} - determine default field and use that
                // {relationfield.childrelationfield.childfield}

                if (!paths.ContainsKey(m.Value))
                {
                    string key = m.Value.TrimStart('{').TrimEnd('}');
                    string[] parts = key.Split('.');

                    var subject = Subject;
                    var path = new FieldPath();
                    foreach (var part in parts)
                    {
                        var field = subject[part];
                        if (field is IRelationField)
                            subject = ((IRelationField)field).RelatedSubject;
                        else if (field == null)
                            break;

                        path.Add(field);
                    }

                    if (path.Count > 0)
                    {
                        if (path.Last is IRelationField)
                            path.Add(FieldPath.FromDefault(path.Last)[1, null]);

                        paths.Add(key, path);
                    }
                }
            }

            return paths;
        }


        /// <summary>
        /// Replaces the placeholders in the given string with data from this item. The replaced data will honour the relevant field's DisplayFormat too.
        /// e.g. "A string with {RelationField1.Field2}" and data{ "RelationField1.Field2": "db data" } returns "A string with db data"
        /// </summary>
        public string ReplacePlaceholders(string placeholderText, Dictionary<string, object> data)
        {
            // query the placeholder dictionary so we have context for formatting values
            var placeholders = GetPlaceholders(placeholderText);
            return PLACEHOLDER_RE.Replace(placeholderText,
                new MatchEvaluator((m) =>
                {
                    // m.Value contains the field name with brackets
                    string fieldName = m.Value.TrimStart('{').TrimEnd('}');

                    if (placeholders.ContainsKey(fieldName) && data.ContainsKey(fieldName))
                    {
                        var field = placeholders[fieldName];
                        var value = data[fieldName];
                        if (value == null)
                            return String.Empty;
                        else if (!String.IsNullOrEmpty(field.Last.DisplayFormat))
                            return String.Format(String.Concat("{0:", field.Last.DisplayFormat, "}"), value);

                        return value.ToString();
                    }

                    return String.Empty;
                }));
        }

        private void RegisterNode(ITemplateTreeNode item)
        {
            if (!Contains(item))
            {
                var subjectNode = item as SubjectTemplateTreeNode;
                if (subjectNode != null)
                    subjectNode.DataSourceLoad += SubjectTemplateTreeNode_DataSourceLoad;
            }
        }

        private void UnregisterNode(ITemplateTreeNode item)
        {
            var subjectNode = item as SubjectTemplateTreeNode;
            if (subjectNode != null)
                subjectNode.DataSourceLoad -= SubjectTemplateTreeNode_DataSourceLoad;
        }

        public override void Add(ITemplateTreeNode item)
        {
            RegisterNode(item);
            base.Add(item);
        }

        public override void Insert(int index, ITemplateTreeNode item)
        {
            RegisterNode(item);
            base.Insert(index, item);
        }

        public override bool Remove(ITemplateTreeNode item)
        {
            UnregisterNode(item);
            return base.Remove(item);
        }

        private void SubjectTemplateTreeNode_DataSourceLoad(object sender, Events.DataSourceLoadEventArgs e)
        {
            // bubble the child events up
            DataSourceLoad?.Invoke(sender, e);
        }

        public override string ToString()
        {
            return $"{Subject?.DisplayName} \"{Text}\"";
        }

        // TODO: re-implement some of the features below after initial development

        ///// <summary>
        ///// Gets or sets the additional fields that will be retrieved when a data node is created from this template.  Note: fields already specified in the Text property will be available.
        ///// </summary>
        //[XmlArray]
        //public List<string> AdditionalFields
        //{
        //	get { return _additional; }
        //	set { _additional = value; }
        //}
        //private List<string> _additional;

        ///// <summary>
        ///// Gets or sets the fields that will be used to create hierarchy based on grouping of data.  This is irrelevant for static nodes.
        ///// </summary>
        //[XmlArray]
        //public List<SortField> GroupBy
        //{
        //	get { return _groups; }
        //	set { _groups = value; }
        //}
        //private List<SortField> _groups;

        ///// <summary>
        ///// Gets or sets the list of fields to sort the data by.
        ///// </summary>
        //[XmlArray]
        //public List<SortField> SortBy
        //{
        //    get { return _sortBy; }
        //    set { _sortBy = value; }
        //}
        //private List<SortField> _sortBy;
    }
}