﻿using dbqf.Criterion.Builders;
using dbqf.Serialization.Assemblers.Criterion;
using dbqf.Serialization.DTO.Builders;

namespace dbqf.Serialization.Assemblers.Builders
{
    /// <summary>
    /// Handles conversion for ParameterBuilders that have no additional properties apart from base class property Label.
    /// This will work for Between, Null, and all DateXBuilders.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class BuilderAssembler<T, U> : BuilderAssembler
        where T : IParameterBuilder, new()
        where U : ParameterBuilderDTO, new()
    {
        public BuilderAssembler(BuilderAssembler successor = null)
            : base(successor)
        {
        }

        public override IParameterBuilder Restore(ParameterBuilderDTO dto)
        {
            var sb = dto as U;
            if (sb == null)
                return base.Restore(dto);

            return new T()
            {
                Label = sb.Label
            };
        }

        public override ParameterBuilderDTO Create(IParameterBuilder b)
        {
            if (!(b is T))
                return base.Create(b);

            return new U()
            {
                Label = ((T)b).Label
            };
        }
    }
}
