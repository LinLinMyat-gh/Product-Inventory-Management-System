using System;
using System.ComponentModel.DataAnnotations;

namespace ProductAssignment.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        // Name is required and has a max length
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters.")]
        public string Name { get; set; } = "";

        // Description can be nullable
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        // BuyingPrice cannot be null or zero
        [Range(0.01, double.MaxValue, ErrorMessage = "Buying price must be greater than zero.")]
        public float? BuyingPrice { get; set; }

        // Supplier is optional, but has a length limit
        [StringLength(100, ErrorMessage = "Supplier name cannot be longer than 100 characters.")]
        public string? Supplier { get; set; }

        // Change to DateTime? if actual dates are needed
        public string? ManufacturingDate { get; set; }
        public string? PurchasingDate { get; set; }

        // ExpiredDate  nullable for the user side
        public string? ExpiredDate { get; set; }

        // ImageFilename can be nullable
        public string? ImageFilename { get; set; }

        
       
    }
}
