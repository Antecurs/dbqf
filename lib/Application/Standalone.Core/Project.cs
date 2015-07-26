﻿using dbqf.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Standalone.Core
{
    public class Project
    {
        /// <summary>
        /// Gets or sets an ID to record the current connection for the next session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version of this project file.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets a name for this project, as shown in the application title bar.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a number of connections that can be used with this configuration.
        /// </summary>
        public List<Connection> Connections { get; set; }

        /// <summary>
        /// Gets or sets the configuration that will be searched.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        private Connection _current;
        public Connection CurrentConnection 
        {
            get 
            {
                if (_current == null)
                    _current = Connections[0];
                return _current;
            }
            set
            {
                _current = value;
                if (CurrentConnectionChanged != null)
                    CurrentConnectionChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler CurrentConnectionChanged;

        public Project()
        {
            Connections = new List<Connection>();
        }
    }

    [DebuggerDisplay("{DisplayName} ({Identifier} | {ConnectionType})")]
    public class Connection
    {
        /// <summary>
        /// Gets or sets a name to display to the user to identify this connection.
        /// </summary>
        [XmlAttribute]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a textual identifier for the connection.
        /// </summary>
        [XmlAttribute]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets a named identifier registered in the IoC container for the type of connection to use.
        /// </summary>
        [XmlAttribute("Type")]
        public string ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets the connection string to use with the corresponding type of connection.
        /// </summary>
        [XmlText]
        public string ConnectionString { get; set; }
    }
}
