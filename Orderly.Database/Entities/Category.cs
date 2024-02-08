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

namespace Orderly.Database.Entities
{
    public partial class Category : BindableBase
    {
        private string name = string.Empty;
        private string flairColor = "#ffff7b2e";

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

        public virtual ICollection<Credential>? Credentials { get; set; }
    }
}
