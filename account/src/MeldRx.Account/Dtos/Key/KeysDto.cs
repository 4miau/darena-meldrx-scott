


namespace DarenaSolutions.Iam.Server.Dtos.Key
{
    public class KeysDto
    {
		public KeysDto()
        {
            Keys = new List<KeyDto>();
        }

        public List<KeyDto> Keys { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
	}
}