using System.Linq;
using Application.Base;
using Application.Models;
using Domain.Contracts;
using Domain.Entities;

namespace Application.Services
{
    public class ConductorService : Service<Conductor>
    {
        private readonly PersonaService _personaService;
        public ConductorService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.ConductorRepository)
        {
            _personaService = new PersonaService(_unitOfWork);
        }

        public ConductorResponse Add(PersonaDerivadoRequest request)
        {
            Conductor entity = new Conductor();

            Persona persona = _unitOfWork.PersonaRepository.FindFirstOrDefault(x => x.Documento.Numero == request.NumeroDocumento);

            if (persona == null)
            {
                return new ConductorResponse($"La persona identificada con documento {request.NumeroDocumento} no existe");
            }

            base.Add(entity);
            _unitOfWork.Commit();

            if (entity.Id == 0)
            {
                return new ConductorResponse("Conductor no registrado");
            }
            return new ConductorResponse("Conductor registrado", entity);
        }

        public ConductorResponse ConPersona(ConPersonaRequest request)
        {
            Persona persona = _unitOfWork.PersonaRepository.FindFirstOrDefault(x => x.Documento.Numero == request.Persona.NumeroDocumento);

            if (persona != null)
            {
                return new ConductorResponse("Ya existe la persona con este número de documento");
            }

            var personaResponse = _personaService.Add(request.Persona);

            if (personaResponse.Entidades == null)
            {
                return new ConductorResponse(personaResponse.Mensaje);
            }

            var conductorRequest = new PersonaDerivadoRequest { NumeroDocumento = request.Persona.NumeroDocumento };
            return Add(conductorRequest);
        }
    }
}