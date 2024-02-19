using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orderly.Database.Modules;

namespace Orderly.Database.Entities
{
    public partial class Credential : BindableBase
    {
        private string serviceName = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;
        private bool pinned = false;
        private bool isEditing = false;
        private string additionDate = string.Empty;
        private string lastEditDate = string.Empty;
        private bool isVisible = true;

        private Category? category;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ServiceName
        {
            get => serviceName;
            set
            {
                SetProperty(ref serviceName, value);
            }
        }
        public string Username
        {
            get => username;
            set
            {
                SetProperty(ref username, value);
            }
        }
        public string Password
        {
            get => password;
            set
            {
                SetProperty(ref password, value);
            }
        }
        public bool Pinned
        {
            get => pinned;
            set
            {
                SetProperty(ref pinned, value);
            }
        }
        public string AdditionDate
        {
            get => additionDate;
            set
            {
                SetProperty(ref additionDate, value);   
            }
        }
        public string LastEditDate
        {
            get => lastEditDate;
            set
            {
                SetProperty(ref lastEditDate, value);
            }
        }
       
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category
        {
            get => category;
            set
            {
                SetProperty(ref category, value);   
            }
        }

        [NotMapped]
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                SetProperty(ref isEditing, value); 
            }
        }
        [NotMapped]
        public bool IsVisibile
        {
            get => isVisible;
            set
            {
                SetProperty(ref isVisible, value);
            }
        }
    }
}
