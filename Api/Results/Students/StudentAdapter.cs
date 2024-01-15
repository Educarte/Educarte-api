using Core;
using Mapster;

namespace Api.Results.Students
{
    /// <summary>
    /// 
    /// </summary>
    public class StudentAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Student, StudentResult>();
        }
    }
}
