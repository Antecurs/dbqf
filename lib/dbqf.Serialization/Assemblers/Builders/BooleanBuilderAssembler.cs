﻿using dbqf.Criterion.Builders;
using dbqf.Serialization.DTO.Builders;
using dbqf.Sql.Criterion.Builders;

namespace dbqf.Serialization.Assemblers.Builders
{
    public class BooleanBuilderAssembler : BuilderAssembler
    {
        public BooleanBuilderAssembler(BuilderAssembler successor = null)
            : base(successor)
        {
        }

        public override IParameterBuilder Restore(ParameterBuilderDTO dto)
        {
            var sb = dto as BooleanBuilderDTO;
            if (sb == null)
                return base.Restore(dto);

            return new BooleanBuilder()
            {
                Label = sb.Label,
                Value = sb.Value
            };
        }

        public override ParameterBuilderDTO Create(IParameterBuilder b)
        {
            var sb = b as BooleanBuilder;
            if (sb == null)
                return base.Create(b);

            return new BooleanBuilderDTO() 
            { 
                Label = sb.Label,
                Value = sb.Value
            };
        }
    }
}
