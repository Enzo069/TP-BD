namespace ParcialWebAPI.Dtos
{
    public class CreateTaskDto
    {
        public int Tokens { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
