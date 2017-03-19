using NUnit.Framework;
using System.IO;

namespace TagLib.Tests.FileFormats
{   
	[TestFixture]
	public class AudibleFormatTest
	{
		private static string BaseDirectory = "samples/audible";

		[Test]
		public void First ()
		{
			using (var file = File.Create(Path.Combine(BaseDirectory, "first.aa"))) {
				var tag = (Audible.Tag)file.Tag;
				Assert.AreEqual(tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
				Assert.AreEqual(tag.Author, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
				Assert.AreEqual(tag.Copyright, "&#169;2009 Ricky Gervais; (P)2009 Ricky Gervais");
				Assert.IsTrue(tag.Description.StartsWith(
					              "This is the second in a new series of definitive discourses exploring the diversity of human"));
				Assert.AreEqual(tag.Narrator, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
				Assert.AreEqual(tag.Title, "The Ricky Gervais Guide to... NATURAL HISTORY (Unabridged)");
			}
		}

		[Test]
		[Ignore ("Not supported yet")]
		public void Second ()
		{
			using (var file = File.Create(Path.Combine(BaseDirectory, "second.aax"))) {
				var tag = (Audible.Tag)file.Tag;
				Assert.AreEqual(tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
				Assert.AreEqual(tag.Author, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
				Assert.AreEqual(tag.Copyright, "&#169;2009 Ricky Gervais; (P)2009 Ricky Gervais");
				Assert.IsTrue(tag.Description.StartsWith(
					              "This is the second in a new series of definitive discourses exploring the diversity of human"));
				Assert.AreEqual(tag.Narrator, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
				Assert.AreEqual(tag.Title, "The Ricky Gervais Guide to... NATURAL HISTORY (Unabridged)");
			}
		}

		[Test]
		public void Third ()
		{
			using (var file = File.Create(Path.Combine(BaseDirectory, "third.aa"))) {
				var tag = (Audible.Tag)file.Tag;
				Assert.AreEqual(tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
				Assert.AreEqual(tag.Author, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
				Assert.AreEqual(tag.Copyright, "&#169;2009 Ricky Gervais; (P)2009 Ricky Gervais");
				Assert.IsTrue(tag.Description.StartsWith(
					              "This is the second in a new series of definitive discourses exploring the diversity of human"));
				Assert.AreEqual(tag.Narrator, "Ricky Gervais, Steve Merchant, & Karl Pilkington");
				Assert.AreEqual(tag.Title, "The Ricky Gervais Guide to... NATURAL HISTORY (Unabridged)");
			}
		}

		[Test]
		public void Fourth ()
		{
			using (var file = File.Create(Path.Combine(BaseDirectory, "fourth.aa"))) {
				var tag = (Audible.Tag)file.Tag;
				Assert.AreEqual(tag.Album, "Glyn Hughes"); // This is probably wrong. The publisher is not the album
				Assert.AreEqual(tag.Author, "Ricky Gervais, Steve Merchant & Karl Pilkington");
				Assert.AreEqual(tag.Copyright, "&#169;2010 Ricky Gervais; (P)2010 Ricky Gervais");
				Assert.IsTrue(tag.Description.StartsWith(
					              "The ninth episode in this new series considers the human body, its form, function, and failings"));
				Assert.AreEqual(tag.Narrator, "Ricky Gervais, Steve Merchant & Karl Pilkington");
				Assert.AreEqual(tag.Title, "The Ricky Gervais Guide to... THE HUMAN BODY");
			}
		}
	}
}
