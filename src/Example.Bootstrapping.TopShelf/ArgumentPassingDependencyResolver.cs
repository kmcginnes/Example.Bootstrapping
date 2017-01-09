using System;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Resolvers;

namespace Example.Bootstrapping.TopShelf
{
    /// <summary>
    /// Passes inline arguments down the chain
    /// 
    /// http://stackoverflow.com/a/4380943/1558906
    /// </summary>
    public class ArgumentPassingDependencyResolver : DefaultDependencyResolver
    {
        protected override CreationContext RebuildContextForParameter(
            CreationContext current, Type parameterType)
        {
            if (parameterType.ContainsGenericParameters)
            {
                // this behaviour copied from base class
                return current;
            }

            // the difference in the following line is that "true" is passed
            // instead of "false" as the third parameter
            return new CreationContext(parameterType, current, true);
        }
    }
}