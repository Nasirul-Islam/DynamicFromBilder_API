namespace DynamicFromBilderAPI.Models
{
	public class CodeBook
	{
		public int CodeId { get; set; }
		public string InputType { get; set; }
		public string DefaultLabel { get; set; }
		public string Description { get; set; }
	}
    public class FormFieldDto
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public string IsRequired { get; set; }
        public string Options { get; set; }   
    }

    public class FormSubmitDto
    {
        public string Title { get; set; }
        public List<FormFieldDto> Fields { get; set; }
    }

}
