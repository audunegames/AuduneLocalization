using Audune.Utils.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines a localized string reference
  [Serializable]
  public class LocalizedString : ILocalizedReference<string>, IEquatable<LocalizedString>
  {
    // Localized string properties
    [SerializeField, Tooltip("The path of the reference, in case the reference is a localized reference")]
    private string _path;
    [SerializeField, Tooltip("The value of the reference, in case the reference is a non-localized reference")]
    private string _value;

    // Internal state of the localized string
    private SerializableDictionary<string, object> _arguments;


    // Return the path of the localized string
    public string path => _path;

    // Return the value of the localized string
    public string value => _value;

    // Return the arguments of the localized string
    public IReadOnlyDictionary<string, object> arguments => _arguments;

    // Return if the string is empty
    public bool isEmpty => string.IsNullOrEmpty(_path) && string.IsNullOrEmpty(_value);


    // Private constructor
    private LocalizedString()
    {
      _path = null;
      _value = null;
      _arguments = new SerializableDictionary<string, object>();
    }

    // Private constructor that copies the values from another localized string
    private LocalizedString(LocalizedString localizedString)
    {
      _path = localizedString._path;
      _value = localizedString._value;
      _arguments = new SerializableDictionary<string, object>(localizedString._arguments);
    }


    // Return if the reference can be resolved
    public bool TryResolve(ILocalizedTable<string> table, out string value)
    {
      if (!string.IsNullOrEmpty(_path))
        return table.TryFind(_path, out value);

      value = _value;
      return true;
    }


    // Return the string representation of the localized string
    public override string ToString()
    {
      var builder = new StringBuilder();
      if (!string.IsNullOrEmpty(_path))
        builder.Append(_path);
      else
        builder.Append($"<Non-Localized Value: \"{_value}\">");
      if (_arguments.Count > 0)
        builder.Append($" with arguments {{{string.Join(", ", _arguments.Select(e => $"{e.Key} = {e.Value}"))}}}");
      return builder.ToString();
    }


    #region Field management
    // Return a new localized string with the specified path
    public LocalizedString WithPath(string path)
    {
      var newReference = new LocalizedString(this);
      newReference._path = path;
      newReference._value = null;
      return newReference;
    }

    // Return a new localized string with the specified value
    public LocalizedString WithValue(string value)
    {
      var newReference = new LocalizedString(this);
      newReference._path = null;
      newReference._value = value;
      return newReference;
    }

    // Return a new localized string with the specified argument
    public LocalizedString WithArgument(string key, object value)
    {
      var newReference = new LocalizedString(this);
      newReference._arguments[key] = value;
      return newReference;
    }

    // Return a new localized string with the specified arguments
    public LocalizedString WithArguments(IDictionary<string, object> arguments)
    {
      var newReference = new LocalizedString(this);
      foreach (var e in arguments)
        newReference._arguments[e.Key] = e.Value;
      return newReference;
    }

    // Return a new localized string without the specified argument
    public LocalizedString WithoutArgument(string key)
    {
      var newReference = new LocalizedString(this);
      newReference._arguments.Remove(key);
      return newReference;
    }

    // Return a new localized string without the specified arguments
    public LocalizedString WithoutArguments(IEnumerable<string> arguments)
    {
      var newReference = new LocalizedString(this);
      foreach (var key in arguments)
        newReference._arguments.Remove(key);
      return newReference;
    }
    #endregion

    #region Equatable implementation
    // Return if the localized string equals another object
    public override bool Equals(object obj)
    {
      return Equals(obj as LocalizedString);
    }

    // Return if the localized string equals another localized string
    public bool Equals(LocalizedString other)
    {
      return other is not null && _path == other._path && _value == other._value && EqualityComparer<SerializableDictionary<string, object>>.Default.Equals(_arguments, other._arguments);
    }

    // Return the hash code of the localized string
    public override int GetHashCode()
    {
      return HashCode.Combine(_path, _value, _arguments);
    }
    #endregion

    #region Equality operators
    // Return if the localized string equals another localized string
    public static bool operator ==(LocalizedString left, LocalizedString right)
    {
      return EqualityComparer<LocalizedString>.Default.Equals(left, right);
    }

    // Return if the localized string does not equal another localized string
    public static bool operator !=(LocalizedString left, LocalizedString right)
    {
      return !(left == right);
    }
    #endregion

    #region Implicit operators
    // Convert a value to a non-localized string
    public static implicit operator LocalizedString(string value)
    {
      return new LocalizedString().WithValue(value);
    }
    #endregion
  }
}
