using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute.Core;
using NSubstitute.Core.DependencyInjection;
using NSubstitute.Proxies;

namespace NSubstitute.Community.Protected
{
    public static class ProtectedExtensions
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
        
        // public static object NonPublicMethod<T>(this T @this, string memberName) where T : class
        // {
        //     return @this.GetType()
        //         .InvokeMember(memberName,
        //             BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
        //             Type.DefaultBinder,
        //             @this,
        //             new object[0]);
        // }

        public static TPrivateInterface Protected<TPrivateInterface>(this object @this) where TPrivateInterface : class
        {
            var result = ProxyGenerator.CreateInterfaceProxyWithoutTarget(typeof(TPrivateInterface),
                new Type[] {typeof(ICallRouterProvider)}, new NonPublicInterceptor(@this));

            return (TPrivateInterface) result;
            //
            //
            // return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TPrivateInterface>(options, new NonPublicInterceptor(@this));
        }
    }
    
    internal class NonPublicInterceptor : IInterceptor
    {
        private readonly object _substitute;

        public NonPublicInterceptor(object substitute)
        {
            _substitute = substitute;
        }
        
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType == typeof(ICallRouterProvider) &&
                invocation.Method.Name == nameof(ICallRouterProvider.GetCallRouter))
            {
                invocation.ReturnValue = (_substitute as ICallRouterProvider)?.GetCallRouter();
                return;
            }
 
            var method = _substitute.GetType().GetMethod(invocation.Method.Name, BindingFlags.NonPublic | BindingFlags.Instance);
            invocation.ReturnValue = method.Invoke(_substitute, invocation.Arguments);
        }
    }
}
