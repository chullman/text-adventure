using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using TextAdventure.Command.Displayables.Models;

namespace TextAdventure.Command.NLP.ModelValidation.CustomRules
{
    public class ListMustNotBeEmpty<T> : PropertyValidator
    {
        public ListMustNotBeEmpty(IDisplayables errorDisplayables)
            : base(errorDisplayables.GetDisplayable<ErrorDisplayables.CommandValidationErrors>().emptyCommand)
        {

        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var list = context.PropertyValue as IList<T>;

            if (list == null || list.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
