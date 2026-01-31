// <copyright file="Field.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests.Helpers;

using System.Reflection;

/// <summary>
/// Provides helper methods to get and set non-public instance fields on objects.
/// </summary>
public static class Field
{
    /// <summary>
    /// Gets the value of a non-public instance field from the given object.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance containing the field.</typeparam>
    /// <typeparam name="TFieldType">The expected type of the field value.</typeparam>
    /// <param name="name">The name of the field to read.</param>
    /// <param name="instance">The instance containing the field.</param>
    /// <returns>
    /// The field value cast to <typeparamref name="TFieldType"/>.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the field does not exist or does not match the expected type.
    /// </exception>
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

    /// <summary>
    /// Sets the value of a non-public instance field on the given object.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance containing the field.</typeparam>
    /// <typeparam name="TFieldType">The expected type of the field value.</typeparam>
    /// <param name="name">The name of the field to write.</param>
    /// <param name="instance">The instance containing the field.</param>
    /// <param name="value">The value to assign to the field.</param>
    /// <exception cref="Exception">
    /// Thrown when the field does not exist or does not match the expected type.
    /// </exception>
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
