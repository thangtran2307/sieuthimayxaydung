using Marketplace.Application.Common;

namespace Marketplace.Application.Tests.Common;

public sealed class SlugTests
{
    [Fact]
    public void Strips_vietnamese_diacritics_and_lowercases()
    {
        string slug = Slug.Generate("Máy đào Komatsu PC200");

        Assert.StartsWith("may-dao-komatsu-pc200-", slug);
    }

    [Theory]
    // Covers every Vietnamese diacritic family: acute/grave/hook/tilde/dot tones plus the
    // circumflex (â ê ô), breve (ă), horn (ơ ư) and the stroked đ.
    [InlineData("Ô tô nâng ưu tiên", "o-to-nang-uu-tien-")]
    [InlineData("Cần cẩu Đặc biệt", "can-cau-dac-biet-")]
    [InlineData("Xe lu rung Hòa Bình", "xe-lu-rung-hoa-binh-")]
    public void Strips_all_vietnamese_tone_and_vowel_marks(string title, string expectedPrefix)
    {
        string slug = Slug.Generate(title);

        Assert.StartsWith(expectedPrefix, slug);
    }

    [Fact]
    public void Collapses_separators_and_trims()
    {
        string slug = Slug.Generate("  Hello --- World!!  ");

        Assert.StartsWith("hello-world-", slug);
        Assert.DoesNotContain("--", slug);
    }

    [Fact]
    public void Falls_back_when_the_title_has_no_usable_characters()
    {
        string slug = Slug.Generate("!!!");

        Assert.StartsWith("listing-", slug);
    }

    [Fact]
    public void Appends_a_unique_suffix()
    {
        Assert.NotEqual(Slug.Generate("Komatsu PC200"), Slug.Generate("Komatsu PC200"));
    }
}
