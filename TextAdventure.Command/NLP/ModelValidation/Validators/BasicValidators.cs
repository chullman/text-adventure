using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using TextAdventure.Command.NLP.Models;

namespace TextAdventure.Command.NLP.ModelValidation.Validators
{
    public class NlpResultRootValidator : AbstractValidator<NlpResult>
    {
    }

    public class NlpResultSentenceValidator : AbstractValidator<Sentence>
    {
    }

    public class NlpResultTokenValidator : AbstractValidator<Token>
    {
    }

    public class NlpResultTokenColValidator : AbstractValidator<IList<Token>>
    {
    }
}
