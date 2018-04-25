using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using TextAdventure.Command.Displayables.Models;
using TextAdventure.Infrastructure.Services.ContentProvider;

namespace TextAdventure.Command.NLP.ModelValidation.CustomRules
{
    public class ListMustContainOnlyOneItem<T> : PropertyValidator
    {
        public ListMustContainOnlyOneItem(IDisplayables errorDisplayables)
            : base(errorDisplayables.GetDisplayable<ErrorDisplayables.CommandValidationErrors>().moreThanOneSentence)
        {
            
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var list = context.PropertyValue as IList<T>;

            if (list != null && list.Count > 1)
            {
                return false;
            }
            return true;
        }
    }
}
