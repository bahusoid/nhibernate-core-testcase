using System;
using System.IO;
using System.Linq;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0000Fluent
{
	/// <summary>
	/// Fixture using FluentNHibernate mappings
	/// </summary>
	[TestFixture]
	public class FluentFixture : TestCase
	{
		private string _hbmMapping;
		protected override string[] Mappings => Array.Empty<string>();

		protected override void AddMappings(Configuration configuration)
		{
			var stringWriter = new StringWriter();
			Fluently.Configure(configuration)
				.Mappings(x =>
					x.FluentMappings
						//.AddFromAssemblyOf<EntityMap>()
						.Add<EntityMap>()
						.ExportTo(stringWriter)
				)
				.BuildConfiguration();
			_hbmMapping = stringWriter.ToString();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void YourTestName()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
					where e.Name == "Bob"
					select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
				transaction.Commit();
			}
		}
	}
}
