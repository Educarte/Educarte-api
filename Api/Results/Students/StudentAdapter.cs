﻿using Core;
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
            config.NewConfig<Student, StudentSoftResult>()
                .Map(x => x.ClassroomName, x => x.Classroom.Name)
                .Map(x => x.ContractedHours, x => x.ContractedHours.FirstOrDefault(x => x.Status == Core.Enums.Status.Active));
            config.NewConfig<Student, StudentSimpleResult>()
                .Map(x => x.ContractedHours, x => x.ContractedHours.Where(x => x.Status == Core.Enums.Status.Active));
        }
    }
}
