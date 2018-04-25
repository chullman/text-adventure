using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using com.sun.jdi.@event;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using TextAdventure.Command.NLP.ModelValidation.CustomRules;

namespace TextAdventure.Command.NLP.ModelValidation
{
    public class BasicValidatorHandler
    {
        private ILifetimeScope _lifetimeScope;

        private Dictionary<object, object> _validatorsForModels = new Dictionary<object, object>();

        public BasicValidatorHandler(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public AbstractValidator<TModel> FetchModel<TModel>(object modelProperty)
        {
            var validatorType = typeof(AbstractValidator<>).MakeGenericType(typeof(TModel));

            var validator = (AbstractValidator<TModel>)_lifetimeScope.Resolve(validatorType);

            foreach (var existingValidatorForModel in _validatorsForModels)
            {
                if (existingValidatorForModel.Key.GetType() == validator.GetType())
                {
                    _validatorsForModels[validator] = modelProperty;
                    return validator;
                }
            }

            _validatorsForModels.Add(validator, modelProperty);

            return validator;
        }


        public ValidationResult ValidateModel(IValidator modelToValidate)
        {
            foreach (var existingValidatorForModel in _validatorsForModels)
            {
                if (existingValidatorForModel.Key.GetType() == modelToValidate.GetType())
                {
                    return modelToValidate.Validate((dynamic)existingValidatorForModel.Value);


                }
            }
            throw new Exception("ERROR: Validator not found.");

        }

        
        public T GetAValidator<T>(IValidator validator, string memberName)
        {
            var metaHook = validator.CreateDescriptor();

            var membersWithValidators = metaHook.GetMembersWithValidators();

            foreach (var propertyValidator in membersWithValidators.First(m => m.Key == memberName))
            {
                if (propertyValidator.GetType() == typeof(T))
                {
                    return (T)propertyValidator;
                }
            }


            throw new NullReferenceException();
        }

        public T GetAValidatorOfEnumerable<T, TU>(AbstractValidator<IList<TU>> validatorCol, int valRulePos)
        {

            var validatorColAsList = validatorCol.AsEnumerable().ToList();

            foreach (var propertyValidator in validatorColAsList[valRulePos].Validators)
            {
                if (propertyValidator.GetType() == typeof(T))
                {
                    return (T)propertyValidator;
                }
            }

            throw new NullReferenceException();

        }
        
    }
}
