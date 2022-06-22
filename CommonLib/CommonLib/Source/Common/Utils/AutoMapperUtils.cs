using System;
using AutoMapper;
using AutoMapper.Configuration;

namespace CommonLib.Source.Common.Utils
{
    public sealed class AutoMapperUtils
    {
        private static Lazy<IMapper> _lazyMapper;
        private static readonly Lazy<AutoMapperUtils> _instance = new Lazy<AutoMapperUtils>(() => new AutoMapperUtils());
        public static AutoMapperUtils Instance => _instance.Value;
        public IMapper Mapper => _lazyMapper?.Value;

        private AutoMapperUtils() { }

        public AutoMapperUtils Configure(Action<IMapperConfigurationExpression> configureUserMapping)
        {
            _lazyMapper = new Lazy<IMapper>(() =>
            {
                var config = new MapperConfiguration(configureUserMapping);
                return config.CreateMapper();
            });
            return this;
        }
    }
}

// USAGE:
//var mapper = AutoMapperUtils.Instance.Configure(cfg =>
//{
//    cfg.CreateMap<int, object>();
//}).Mapper;

//var ttt = mapper.Map(0, new object());
//// OR
//var tt = 0.Map(new object());
