using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Orderly.Database.Modules;
using System.Collections.ObjectModel;

namespace Orderly.Database.Entities
{
    public partial class Category : BindableBase
    {
        private string name = string.Empty;
        private string flairColor = "#ffff7b2e";
        private bool isFavorite;
        private string additionDate = string.Empty;
        private string lastEditDate = string.Empty;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
            }
        }
        public string FlairColor
        {
            get => flairColor;
            set
            {
                SetProperty(ref flairColor, value); 
            }
        }
        public bool IsFavorite
        {
            get => isFavorite;
            set
            {
                SetProperty(ref isFavorite, value);
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

        public virtual ICollection<Credential>? Credentials { get; set; } = new List<Credential>();
    }
}
