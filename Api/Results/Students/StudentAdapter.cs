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
            config.NewConfig<Student, StudentResult>()
                .Map(x => x.ContractedHours, x => x.ContractedHours.Where(x => !x.DeletedAt.HasValue));

            config.NewConfig<Student, StudentBasicResult>();
            config.NewConfig<Student, StudentSoftResult>()
                .Map(x => x.ContractedHours, x => x.ContractedHours.FirstOrDefault(x => x.Status == Core.Enums.Status.Active));
            config.NewConfig<Student, StudentSimpleResult>()
                .Map(x => x.ContractedHours, x => x.ContractedHours.Where(x => x.Status == Core.Enums.Status.Active && !x.DeletedAt.HasValue))
                .Map(x => x.LegalGuardian, x => x.LegalGuardian)
                .Map(x => x.AccessControls, x => x.AccessControls.Where(x => x.Time.Date >= DateTime.Now.Date && !x.DeletedAt.HasValue));

            config.NewConfig<Student, StudentSimpleListResult>();
        }
    }
}
