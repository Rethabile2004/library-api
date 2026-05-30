namespace LibraryApi.Exceptions
{
    // Thrown when a requested resource does not exist
    // middleware catches this and returns 404
    public class NotFoundException:Exception
    {
        public NotFoundException(string message) : base(message) { }
        // builds standard message from entity name and id
        public NotFoundException(string entityName, int id) : base($"{entityName} with id {id} was not found.") { }
    }
}
