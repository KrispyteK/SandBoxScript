﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Skrypt {
    public class StringInstance : BaseInstance, IValue {
        public override string Name => "String";

        public string Value { get; set; }

        public StringInstance(Engine engine, string value) : base(engine) {
            Value = value;
        }

        public BaseObject Get(int index) {
            if (index >= 0 && index < Value.Length) {
                return Engine.CreateString(Value[index].ToString());
            }

            return Engine.CreateString(string.Empty);
        }

        public BaseObject Get(BaseObject index) {
            if (index is NumberInstance number && number >= 0 && number % 1 == 0) {
                return Get((int)number);
            }

            return Engine.CreateString(string.Empty);
        }

        public static BaseObject Length(Engine engine, BaseObject self) {
            return engine.CreateNumber((self as StringInstance).Value.Length);
        }

        public static BaseObject Substring (Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var start = arguments.GetAs<NumberInstance>(0);
            var end = arguments[1];
            
            if (end == null) {
                return engine.CreateString(str.Value.Substring((int)start));
            } else {
                if (end is NumberInstance) {
                    var length = Math.Max(Math.Min((int)(NumberInstance)end, str.Value.Length - 1) - (int)start, 0);

                    return engine.CreateString(str.Value.Substring((int)start, length));
                } else {
                    throw new InvalidArgumentTypeException($"Expected argument of type Number.");
                }
            }
        }

        public static BaseObject IndexOf(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var search = arguments.GetAs<StringInstance>(0);
            var start = arguments[1];

            if (start == null) {
                return engine.CreateNumber(str.Value.IndexOf(search));
            }
            else {
                if (start is NumberInstance num) {
                    var startIndex = Math.Max(Math.Min((int)num, str.Value.Length - 1), 0);

                    return engine.CreateNumber(str.Value.IndexOf(search,startIndex));
                }
                else {
                    throw new InvalidArgumentTypeException($"Expected argument of type Number.");
                }
            }
        }

        public static BaseObject Contains(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var input = arguments.GetAs<StringInstance>(0);

            return engine.CreateBoolean(str.Value.Contains(input.Value));
        }

        public static BaseObject StartsWith(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var input = arguments.GetAs<StringInstance>(0);

            return engine.CreateBoolean(str.Value.StartsWith(input.Value));
        }

        public static BaseObject EndsWith(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var input = arguments.GetAs<StringInstance>(0);

            return engine.CreateBoolean(str.Value.EndsWith(input.Value));
        }

        public static BaseObject Replace(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var input = arguments.GetAs<StringInstance>(0);
            var replacement = arguments.GetAs<StringInstance>(1);

            return engine.CreateString(str.Value.Replace(input.Value, replacement.Value));
        }

        public static BaseObject ReplaceRE(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var input = arguments.GetAs<StringInstance>(0);
            var replacement = arguments.GetAs<StringInstance>(1);

            var newString = Regex.Replace(str.Value, input.Value, replacement.Value);

            return engine.CreateString(newString);
        }

        public static BaseObject PadLeft(Engine engine, BaseObject self, Arguments arguments) {
            var str = (self as StringInstance).Value;
            var totalWidth = arguments.GetAs<NumberInstance>(0);
            var input = arguments.GetAs<StringInstance>(1);
            var newStr = "";

            while (newStr.Length < totalWidth) {
                newStr = input + newStr;
            }

            return engine.CreateString(newStr + str);
        }

        public static BaseObject PadRight(Engine engine, BaseObject self, Arguments arguments) {
            var str = (self as StringInstance).Value;
            var totalWidth = arguments.GetAs<NumberInstance>(0);
            var input = arguments.GetAs<StringInstance>(1);
            var newStr = "";

            while (newStr.Length < totalWidth) {
                newStr = newStr + input;
            }

            return engine.CreateString(str + newStr);
        }

        public static BaseObject ToByteArray(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            var bytes = Encoding.ASCII.GetBytes(str.Value);
            var array = engine.CreateArray(new BaseObject[0]);

            for (var i = 0; i < bytes.Length; i++) {
                ArrayInstance.Push(engine, array, new Arguments(engine.CreateNumber(bytes[i])));
            }

            return array;
        }

        public static BaseObject Reverse(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            char[] charArray = str.Value.ToCharArray();
            Array.Reverse(charArray);

            return engine.CreateString(new string(charArray));
        }

        public static BaseObject ToUpper(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            return engine.CreateString(str.Value.ToUpper());
        }

        public static BaseObject ToLower(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            return engine.CreateString(str.Value.ToLower());
        }

        public static BaseObject Trim(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            return engine.CreateString(str.Value.Trim());
        }

        public static BaseObject TrimStart(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            return engine.CreateString(str.Value.TrimStart());
        }

        public static BaseObject TrimEnd(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            return engine.CreateString(str.Value.TrimEnd());
        }

        public static BaseObject ToArray(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;

            var charArray = str.Value.ToCharArray();
            var array = engine.CreateArray(new BaseObject[0]);

            for (var i = 0; i < charArray.Length; i++) {
                ArrayInstance.Push(engine, array, new Arguments(engine.CreateString("" + charArray[i])));
            }

            return array;
        }

        public static BaseObject Split(Engine engine, BaseObject self, Arguments arguments) {
            var str = self as StringInstance;
            var input = new List<string>();

            for (int i = 0; i < arguments.Values.Length; i++) {
                var s = arguments.GetAs<StringInstance>(i);

                input.Add(s.Value);
            }

            var split = str.Value.Split(input.ToArray(), StringSplitOptions.None);

            var stringArray = new StringInstance[split.Length];

            for (int i = 0; i < split.Length; i++) {
                stringArray[i] = engine.CreateString(split[i]);
            }

            return engine.CreateArray(stringArray);
        }

        public static implicit operator string(StringInstance s) {
            return s.Value;
        }

        public override string ToString() {
            return Value.ToString();
        }

        public BaseObject Copy() {
            return Engine.CreateString(Value);
        }

        bool IValue.Equals(IValue other) {
            return this.Value.Equals(((StringInstance)other).Value);
        }
    }
}
