namespace DynamicFromBilderAPI.Models
{
    public class FormFields
    {
        public int FieldId { get; set; }  
        public int FormId { get; set; }    
        public string Label { get; set; } 
        public string FieldType { get; set; }  
        public bool IsRequired { get; set; }   
    }
}
