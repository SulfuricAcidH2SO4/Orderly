using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Orderly.Extensions
{
    public class EnumToItemSourceExtension : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemSourceExtension(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_type);
        }
    }
}
