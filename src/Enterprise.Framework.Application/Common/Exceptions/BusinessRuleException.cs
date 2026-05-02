namespace Enterprise.Framework.Application.Common.Exceptions;

using Enterprise.Framework.Domain.Common;
using System;

public class BusinessRuleException : Exception
{
    public IBusinessRule BrokenRule { get; }
    public string ErrorCode { get; }

    public BusinessRuleException(IBusinessRule brokenRule) : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        ErrorCode = brokenRule.ErrorCode;
    }
}
