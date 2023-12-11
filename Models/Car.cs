using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cars.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Class { get; set; }

        [Required]
        public string ModelName { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9]{10}$", ErrorMessage = "Model Code must be 10 alphanumeric characters.")]
        public string ModelCode { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Features { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive value.")]
        public int Price { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ManufacturingDate { get; set; }

        public bool Active { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive value-.")]
        public int SortOrder { get; set; }


        public string ImageName { get; set; } = string.Empty;

        

        [NotMapped]
        [Required]
        /* [MaxLength(5 * 1024 * 1024)] // 5 MB*/

        [FileSize(5 * 1024 * 1024)] // 5 MB
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        public List<IFormFile> CarImages { get; set; }
    
  
        public Car()
        {
            ImageName = string.Empty;
        }
        public class FileSizeAttribute : ValidationAttribute
        {
            private readonly long _maxFileSize;

            public FileSizeAttribute(long maxFileSize)
            {
                _maxFileSize = maxFileSize;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var files = value as List<IFormFile>;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > _maxFileSize)
                        {
                            return new ValidationResult($"The file {file.FileName} exceeds the maximum allowed file size.");
                        }
                    }
                }

                return ValidationResult.Success;
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class AllowedExtensionsAttribute : ValidationAttribute
        {
            private readonly string[] _extensions;

            public AllowedExtensionsAttribute(string[] extensions)
            {
                _extensions = extensions;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var files = value as List<IFormFile>;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var extension = System.IO.Path.GetExtension(file.FileName);

                        if (!_extensions.Contains(extension.ToLower()))
                        {
                            var allowedExtensions = string.Join(", ", _extensions);
                            return new ValidationResult($"The file {file.FileName} has an invalid extension. Allowed extensions are: {allowedExtensions}");
                        }
                    }
                }

                return ValidationResult.Success;
            }
        }
    }
}

