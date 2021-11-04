using AutoMapper;
using Centromedico.Database.DbModels;
using Cliente.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Profiles
{
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {
            CreateMap<UserInfo, MyIdentityUser>();
            CreateMap<MyIdentityUser, UserInfo>();
            CreateMap<pacientes, MyIdentityUser>()
                .ForMember(dto => dto.Id, opt => opt.Ignore());


            CreateMap<MyIdentityUser, pacientes>()
                .ForMember(dto => dto.ID, opt => opt.Ignore())
                .ForMember(dest => dest.MyIdentityUserID, opt => opt.MapFrom(src => src.Id));
            CreateMap<especialidades, especialidadDTO>();
            CreateMap<seguros, seguroDTO>();


            CreateMap<pruebas, pruebaDTO>()
                .ForMember(dest => dest.descrip, opt => opt.MapFrom(src => src.analisis.descrip));

            CreateMap<pruebaDTO, pruebas>();

            CreateMap<citas, citaResultDTO>()
                .ForMember(dest => dest.cod_verificacion, opt => opt.MapFrom(src => src.cod_verificacionID))
                .ForMember(dest => dest.servicio, opt => opt.MapFrom(src => src.servicios.descrip))
                .ForMember(dest => dest.consultorio, opt => opt.MapFrom(src => src.consultorio))
                .ForMember(dest => dest.fecha_hora, opt => opt.MapFrom(src => src.fecha_hora))
                .ForMember(dest => dest.medico_nombre_apellido, opt => opt.MapFrom(src => (src.medicos.nombre + " " + src.medicos.apellido).Trim()))
                .ForMember(dest => dest.seguro, opt => opt.MapFrom(src => src.seguros.descrip))
                .ForMember(dest => dest.cobertura, opt => opt.MapFrom(src => src.cobertura))
                .ForMember(dest => dest.diferencia, opt => opt.MapFrom(src => src.diferencia))
                .ForMember(dest => dest.doc_identidad, opt => opt.MapFrom(src => src.pacientes.doc_identidad))
                .ForMember(dest => dest.paciente_nombre_apellido, opt => opt.MapFrom(src => (src.pacientes.nombre + " " + src.pacientes.apellido).Trim()))
                .ForMember(dest => dest.doc_identidad_tutor, opt => opt.MapFrom(src => src.pacientes.doc_identidad_tutor))
                .ForMember(dest => dest.tutor_nombre_apellido, opt => opt.MapFrom(src => (src.pacientes.nombre_tutor + " " + src.pacientes.apellido_tutor).Trim()))
                .ForMember(dest => dest.contacto, opt => opt.MapFrom(src => src.contacto))
                .ForMember(dest => dest.correo, opt => opt.MapFrom(src => src.pacientes.MyIdentityUsers.Email))
                .ForMember(dest => dest.turno, opt => opt.MapFrom(src => src.turno));


            CreateMap<citas, citaDTO>()
                .ForMember(dest => dest.medico_nombre, opt => opt.MapFrom(src => src.medicos.nombre))
                .ForMember(dest => dest.medico_apellido, opt => opt.MapFrom(src => src.medicos.apellido))
                .ForMember(dest => dest.paciente_nombre, opt => opt.MapFrom(src => src.pacientes.nombre))
                .ForMember(dest => dest.paciente_apellido, opt => opt.MapFrom(src => src.pacientes.apellido))
                .ForMember(dest => dest.paciente_nombre_tutor, opt => opt.MapFrom(src => src.pacientes.nombre_tutor))
                .ForMember(dest => dest.paciente_apellido_tutor, opt => opt.MapFrom(src => src.pacientes.apellido_tutor))
                .ForMember(dest => dest.servicio_descrip, opt => opt.MapFrom(src => src.servicios.descrip))
                .ForMember(dest => dest.seguro_descrip, opt => opt.MapFrom(src => src.seguros.descrip));
            //  .ForMember(dest => dest.especialidad_descrip, opt => opt.MapFrom(src => src.especialidades.descrip));


            CreateMap<pacientes, pacienteDTO>();
            CreateMap<pacienteDTO, pacientes>();
            CreateMap<citaCreateDTO, pacientes>();
            CreateMap<RegisterDTO, MyIdentityUser>()
               .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));

            CreateMap<medicos, medicoDirectorioDTO>()
                .ForMember(dest => dest.especialidades, opt => opt.MapFrom(src =>
                    src.especialidades_medicos.Select(x => x.especialidades.descrip).ToList()));

            
            CreateMap<servicios, servicioDTO>();
            CreateMap<cobertura_medicos, coberturaMedicoDTO>();
            CreateMap<cobertura_medicos, coberturaDTO>()
                .ForMember(dest => dest.descrip, opt => opt.MapFrom(src => src.seguros.descrip));
            CreateMap<servicios_medicos, servicioDTO>();

            CreateMap<medicos, medicoDTO>()
         .ForMember(dest => dest.especialidades, opt => opt.MapFrom(src =>
             src.especialidades_medicos.Select(x => x.especialidades.descrip).ToList()
             )).ForMember(dest => dest.seguros, opt => opt.MapFrom(src =>
            src.cobertura_medicos.Select(x => x.seguros.descrip).ToList()
            )).ForMember(dest => dest.servicios, opt => opt.MapFrom(src =>
            src.servicios_medicos.Where(x => x.medicos.ID == src.ID).Select(x => x.servicios.descrip).ToList()
            ));
        }
         


}
}
