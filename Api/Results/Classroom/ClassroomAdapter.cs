using Core;
using Mapster;

namespace Api.Results.Classroom
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassroomAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Core.Classroom, ClassroomResult>();
            config.NewConfig<Core.Classroom, ClassroomSimpleResult>();
        }
    }
}
