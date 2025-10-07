using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.MaibaHeroSlider.Domain;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Data;

[NopSchemaMigration("2025/01/01 00:00:00", "Widgets.MaibaHeroSlider schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<HeroSlide>();
    }
}

