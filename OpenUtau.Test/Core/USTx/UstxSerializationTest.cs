﻿using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;
using OpenUtau.Core.Formats;

namespace OpenUtau.Core.Ustx {
    public class UstxSerializationTest {
        readonly ITestOutputHelper output;
        readonly JsonConverter converter;
        readonly UExpressionDescriptor descriptor;
        readonly UNote note;

        public UstxSerializationTest(ITestOutputHelper output) {
            this.output = output;
            converter = new UExpressionConverter();
            descriptor = new UExpressionDescriptor("velocity", "vel", 0, 200, 100);

            note = UNote.Create();
            note.position = 120;
            note.duration = 60;
            note.noteNum = 42;
            note.lyric = "あ";
            note.expressions.Clear();
            var exp = new UExpression(descriptor) { value = 123 };
            note.expressions.Add(descriptor.abbr, exp);
        }

        [Fact]
        public void UExpressionSerializationTest() {
            var exp = new UExpression(descriptor) { value = 123 };

            var actual = JsonConvert.SerializeObject(exp, Formatting.Indented, converter);
            Assert.Equal("123.0", actual);
        }

        [Fact]
        public void UExpressionDeserializationTest() {
            var exp = new UExpression(descriptor) { value = 123 };
            var json = JsonConvert.SerializeObject(exp, Formatting.Indented, converter);
            output.WriteLine(json);

            var actual = JsonConvert.DeserializeObject<UExpression>(json, converter);

            Assert.Null(actual.descriptor);
            Assert.Equal(123, actual.value);
        }

        [Fact]
        public void UNoteSerializationTest() {
            var actual = JsonConvert.SerializeObject(note, Formatting.Indented, converter);
            output.WriteLine(actual);

            string expected = @"{
  'pos': 120,
  'dur': 60,
  'num': 42,
  'lrc': 'あ',
  'pho': [
    {
      'position': 0,
      'phoneme': 'a',
      'preutter': 0.0,
      'overlap': 0.0,
      'envelope': {
        'data': [
          {
            'X': 0.0,
            'Y': 0.0
          },
          {
            'X': 0.0,
            'Y': 100.0
          },
          {
            'X': 0.0,
            'Y': 100.0
          },
          {
            'X': 0.0,
            'Y': 100.0
          },
          {
            'X': 0.0,
            'Y': 0.0
          }
        ]
      }
    }
  ],
  'pit': {
    'data': [],
    'snapFirst': true
  },
  'vbr': {
    'length': 0.0,
    'period': 0.0,
    'depth': 0.0,
    'in': 0.0,
    'out': 0.0,
    'shift': 0.0,
    'drift': 0.0
  },
  'exp': {
    'vel': 123.0
  }
}";

            Assert.Equal(expected.Replace('\'', '\"').Replace("\r\n", "\n"), actual.Replace("\r\n", "\n"));
        }

        [Fact]
        public void UNoteDeserializationTest() {
            var json = JsonConvert.SerializeObject(note, Formatting.Indented, converter);

            var actual = JsonConvert.DeserializeObject<UNote>(json, converter);

            Assert.Equal(120, actual.position);
            Assert.Equal(60, actual.duration);
            Assert.Equal(42, actual.noteNum);
            Assert.Equal("あ", actual.lyric);
            Assert.Single(actual.phonemes);
            Assert.Equal(0, actual.phonemes[0].position);
            Assert.Equal("a", actual.phonemes[0].phoneme);
            Assert.Equal(0, actual.phonemes[0].preutter);
            Assert.Equal(0, actual.phonemes[0].overlap);
            Assert.Single(actual.expressions);
            var vel = actual.expressions["vel"];
            Assert.NotNull(vel);
            Assert.Null(vel.descriptor);
            Assert.Equal(123, vel.value);
        }
    }
}