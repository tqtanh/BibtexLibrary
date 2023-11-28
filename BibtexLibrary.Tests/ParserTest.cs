using System.IO;
using System.Linq;
using BibtexLibrary;
using BibtexLibrary.Parser;
using BibtexLibrary.Tokenizer;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace BibtexImporter.Tests
{
    [TestFixture]
    class ParserTest
    {
        [Test]
        public void SimpleParserText()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = {David A. Aaker}
                                                                            }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(1, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David A. Aaker", file.Entries.First().Tags.First().Value);
        }

        [Test]
        public void MultipleTagsTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = {David A. Aaker},
                                                                                title = {Multivariate statistics}
                                                                            }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(2, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David A. Aaker", file.Entries.First().Tags.First().Value);
            ClassicAssert.AreEqual("title", file.Entries.First().Tags.ToList()[1].Key);
            ClassicAssert.AreEqual("Multivariate statistics", file.Entries.First().Tags.ToList()[1].Value);
        }

        [Test]
        public void BracesInValueTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = {David A. ()ker},
                                                                            }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(1, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David A. ()ker", file.Entries.First().Tags.First().Value);
        }

        [Test]
        public void MultipleEntriesTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = {David A. Aaker},
                                                                                title = {Multivariate statistics}
                                                                            }
                                                                            @book{ baker:1912,
                                                                                author = {David A. Baker},
                                                                                title = {Multivariate statistics 2}
                                                                            }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(2, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(2, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David A. Aaker", file.Entries.First().Tags.First().Value);
            ClassicAssert.AreEqual("title", file.Entries.First().Tags.ToList()[1].Key);
            ClassicAssert.AreEqual("Multivariate statistics", file.Entries.First().Tags.ToList()[1].Value);

            ClassicAssert.AreEqual("baker:1912", file.Entries.ToList()[1].Key);
            ClassicAssert.AreEqual("book", file.Entries.ToList()[1].Type);
            ClassicAssert.AreEqual(2, file.Entries.ToList()[1].Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.ToList()[1].Tags.First().Key);
            ClassicAssert.AreEqual("David A. Baker", file.Entries.ToList()[1].Tags.First().Value);
            ClassicAssert.AreEqual("title", file.Entries.ToList()[1].Tags.ToList()[1].Key);
            ClassicAssert.AreEqual("Multivariate statistics 2", file.Entries.ToList()[1].Tags.ToList()[1].Value);
        }


        [Test]
        public void CommaInTagValueParseTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = {Günther, C.W. and Van Der Aalst, W.M.P.}
                                                                            }");
            
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(1, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("Günther, C.W. and Van Der Aalst, W.M.P.", file.Entries.First().Tags.First().Value);
        }

        [Test]
        public void FuzzyMiningTestFileTest()
        {
            using (StreamReader reader = new StreamReader("Test Files\\Fuzzy Mining.bib"))
            {
                Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), reader.ReadToEnd());
                BibtexParser parser = new BibtexParser(tokenizer);
                BibtexFile file = parser.Parse();

                ClassicAssert.AreEqual(3, file.Entries.Count);
            }
        }

        [Test]
        public void SimpleParserWithDoubleQuoteTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = ""David A. Aaker""
                                                                              }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(1, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David A. Aaker", file.Entries.First().Tags.First().Value);
        }

        [Test]
        public void ParseBracesInQuotedStringTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = ""David {A.} Aaker""
                                                                              }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(1, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David {A.} Aaker", file.Entries.First().Tags.First().Value);
        }

        [Test]
        public void ParseBracesInBracesStringTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"@book{ aaker:1912,
                                                                                author = {David {A.} Aaker}
                                                                              }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual(1, file.Entries.First().Tags.Count);
            ClassicAssert.AreEqual("author", file.Entries.First().Tags.First().Key);
            ClassicAssert.AreEqual("David {A.} Aaker", file.Entries.First().Tags.First().Value);
        }

        [Test]
        public void ParseStringDefinitionsTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"
                                                                            @String{pub-ACM = ""ACM Press""}
                                                                            @book{ aaker:1912,
                                                                                author = { tes(;)est }
                                                                            }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.IsTrue(file.StringDefinitions.ContainsKey("pub-ACM"));
            ClassicAssert.AreEqual("ACM Press", file.StringDefinitions["pub-ACM"]);

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
        }

        /**
        [Test]
        public void ParseUnstartedTagValuesTest()
        {
            Tokenizer tokenizer = new Tokenizer(new ExpressionDictionary(), @"
                                                                            @book{ aaker:1912,
                                                                                year = 1234
                                                                            }");
            BibtexParser parser = new BibtexParser(tokenizer);
            BibtexFile file = parser.Parse();

            ClassicAssert.AreEqual(1, file.Entries.Count);
            ClassicAssert.AreEqual("aaker:1912", file.Entries.First().Key);
            ClassicAssert.AreEqual("book", file.Entries.First().Type);
            ClassicAssert.AreEqual("1234", file.Entries.First().Tags["year"]);
        }
        **/
    }
}
