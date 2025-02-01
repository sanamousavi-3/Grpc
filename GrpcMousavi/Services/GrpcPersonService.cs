using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcMousavi.Model;
using GrpcMousavi.Context;
using static GrpcMousavi.Protos.PersonService;
using GrpcMousavi.Protos;



namespace GrpcMousavi.Services
{
    public class GrpcPersonService : PersonServiceBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public GrpcPersonService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }


        public override async Task<CreatePersonReply> CreatePerson(
         CreatePersonRequest request, ServerCallContext context)
        {
            try
            {
                var person = new Person
                {
                    Name = request.Name,
                    Family = request.Family,
                    NationalId = request.NationalId,
                    DateBirth = request.DateBirth.ToDateTime()
                };

                await _unitOfWork.Persons.AddAsync(person);
                await _unitOfWork.SaveAsync();

                return new CreatePersonReply
                {

                    Message = $"Person {person.Name} {person.Family} created successfully",
                    Status = 200
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error creating person: {ex.Message}"));
            }

        }
        public override async Task<GetPersonByIdReply> GetPersonById(
                 GetPersonByIdRequest request, ServerCallContext context)
        {
            try
            {
                Person? person = await _unitOfWork.Persons.GetByIdAsync(request.Id);

                if (person == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));

                return new GetPersonByIdReply
                {
                    Id = person.Id,
                    Name = person.Name,
                    Family = person.Family,
                    NationalId = person.NationalId,
                    DateBirth = Timestamp.FromDateTime(DateTime.SpecifyKind(person.DateBirth, DateTimeKind.Utc))
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error retrieving person: {ex.Message}"));
            }
        }

        public override async Task<UpdatePersonReply> UpdatePerson(
                                      UpdatePersonRequest request,
                                      ServerCallContext context)
        {
            try
            {
                var person = await _unitOfWork.Persons.GetByIdAsync(request.Id);
                if (person == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));

                person.Name = request.Name;
                person.Family = request.Family;
                person.NationalId = request.NationalId;
                person.DateBirth = request.DateBirth.ToDateTime();

                _unitOfWork.Persons.Update(person);
                await _unitOfWork.SaveAsync();

                return new UpdatePersonReply
                {
                    Status = 200,
                    Message = "Person updated successfully"
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error updating person: {ex.Message}"));
            }
        }


        public override async Task<DeletePersonReply> DeletePerson(
               DeletePersonRequest request,
               ServerCallContext context)
        {
            try
            {
                var person = await _unitOfWork.Persons.GetByIdAsync(request.Id);
                if (person == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));

                _unitOfWork.Persons.Delete(person);
                await _unitOfWork.SaveAsync();

                return new DeletePersonReply
                {
                    Status = 200,
                    Message = "Person deleted successfully"
                };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error deleting person: {ex.Message}"));
            }
        }



        public override async Task GetAllPersons(Protos.Empty request, IServerStreamWriter<GetAllPersonsReply> responseStream, ServerCallContext context)
        {
            try
            {
                var persons = await _unitOfWork.Persons.GetAllAsync();

                foreach (var person in persons)
                {
                    var reply = new GetAllPersonsReply
                    {
                        Id = person.Id,
                        Name = person.Name,
                        Family = person.Family,
                        NationalId = person.NationalId,
                        DateBirth = Timestamp.FromDateTime(DateTime.SpecifyKind(person.DateBirth, DateTimeKind.Utc))
                    };


                    await responseStream.WriteAsync(reply);
                }
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error retrieving persons: {ex.Message}"));
            }
        }



    }
}


