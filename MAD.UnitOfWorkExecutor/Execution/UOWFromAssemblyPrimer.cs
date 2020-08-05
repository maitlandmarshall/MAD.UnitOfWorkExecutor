﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWFromAssemblyPrimer
    {
        private readonly UnitOfWorkFactory unitOfWorkFactory;

        public UOWFromAssemblyPrimer(UnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<UnitOfWork> Prime(Assembly assembly)
        {
            IEnumerable<UnitOfWork> assemblyUnitsOfWork = this.GetUnitOfWorksInAssembly(assembly);

            foreach (var uom in assemblyUnitsOfWork)
            {
                uom.Children = this.GetDependentUnitOfWorks(uom, assemblyUnitsOfWork);

                if (uom.Attribute.WorkType != UnitOfWorkType.Reactive)
                    yield return uom;
            }
        }

        private IEnumerable<UnitOfWork> GetUnitOfWorksInAssembly(Assembly assembly)
        {
            IEnumerable<Type> exportedTypes = assembly.ExportedTypes;

            foreach (Type t in exportedTypes)
            {
                IEnumerable<UnitOfWork> result = t
                    .GetMethods()
                    .Where(y => y.GetCustomAttribute<UnitOfWorkAttribute>() != null)
                    .Select(y => this.unitOfWorkFactory.Create(y));

                foreach (var uom in result)
                {
                    yield return uom;
                }
            }
        }

        private IEnumerable<UnitOfWork> GetDependentUnitOfWorks(UnitOfWork parent, IEnumerable<UnitOfWork> allUnitOfWorks)
        {
            // Does this UnitOfWork return anything important?
            Type returnType = parent.MethodInfo.ReturnType;
            bool isTask = typeof(Task).IsAssignableFrom(returnType);

            if (isTask && returnType.GenericTypeArguments.Length > 0)
            {
                // It is a Task<Something>
                // Set returnType to <Something>
                returnType = returnType.GenericTypeArguments.First();
            }
            else if (isTask)
            {
                // If it's just a normal task, ignore it
                returnType = null;
            }

            if (returnType is null)
                yield break;

            // Search for UnitOfWorks which accept this returnType as a method parameter or class parameter
            var dependants = allUnitOfWorks
                .Where(y => y != parent)
                .Where(y => 
                    y.MethodInfo.GetParameters().Any(param => returnType.IsAssignableFrom(param.ParameterType))
                    || y.MethodInfo.DeclaringType.GetConstructors().Any(constructor => constructor.GetParameters().Any(constParam => returnType.IsAssignableFrom(constParam.ParameterType))));

            foreach (var p in dependants)
            {
                yield return p;
            }
        }
    }
}
