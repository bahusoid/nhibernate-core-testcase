using FluentNHibernate.Mapping;

namespace NHibernate.Test.NHSpecificTest.GH0000Fluent
{
	public class EntityMap : ClassMap<Entity>
	{
		public EntityMap()
		{
			Id(x => x.Id).GeneratedBy.Guid();
			Map(x => x.Name);
		}
	}
}
