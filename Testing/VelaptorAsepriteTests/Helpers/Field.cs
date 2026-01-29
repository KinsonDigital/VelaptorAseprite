// <copyright file="Field.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests.Helpers;

using System.Reflection;

public static class Field
{
    public static TFieldType GetFieldValue<TInstance, TFieldType>(string name, TInstance instance)
    {
        var type = instance.GetType();
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new Exception($"The field {name} does not exist in the instance.");

        var fieldValue = field.GetValue(instance);

        if (fieldValue is TFieldType value)
        {
            return value;
        }

        throw new Exception($"The field {name} does not have the expected type of {typeof(TFieldType)}.");
    }

    public static void SetFieldValue<TInstance, TFieldType>(string name, TInstance instance, TFieldType value)
    {
        var type = instance.GetType();
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new Exception($"The field {name} does not exist in the instance.");

        if (field.FieldType != typeof(TFieldType))
        {
            throw new Exception($"The field {name} does not have the expected type of {typeof(TFieldType)}.");
        }

        field.SetValue(instance, value);
    }
}
