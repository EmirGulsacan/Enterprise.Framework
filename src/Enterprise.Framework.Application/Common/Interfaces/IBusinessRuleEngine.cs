namespace Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Common;



public interface IBusinessRuleEngine {

Task CheckAsync(IBusinessRule rule);

    Task CheckAsync(params IBusinessRule[] rules);
}



