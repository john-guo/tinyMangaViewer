using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tinyMangaViewer
{
    public static class Filters
    {
        public static Dictionary<string, IFilter> All = new Dictionary<string, IFilter>()
        {
            [nameof(NormalFilter)] = new NormalFilter(),
            [nameof(DigitFirstFilter)] = new DigitFirstFilter(),
        };
    }

    public interface IFilter
    {
        IOrderedEnumerable<string> Filter(IEnumerable<string> list);
    }

    public class NormalFilter : IFilter
    {
        public IOrderedEnumerable<string> Filter(IEnumerable<string> list)
        {
            return list.OrderBy(name => Path.GetDirectoryName(name)).ThenBy(name => name);
        }
    }

    public class DigitFirstFilter : IFilter
    {
        private const string _pattern1 = @"^\d+";
        private const string _pattern2 = @"\d+$";
        private static readonly Regex _regex1 = new Regex(_pattern1, RegexOptions.Compiled);
        private static readonly Regex _regex2 = new Regex(_pattern2, RegexOptions.Compiled);

        public IOrderedEnumerable<string> Filter(IEnumerable<string> list)
        {
            return list.OrderBy(name => Path.GetDirectoryName(name))
                        .ThenBy(name =>
                        {
                            var match = _regex1.Match(Path.GetFileNameWithoutExtension(name));
                            if (match.Success)
                            {
                                if (int.TryParse(match.Value, out int i))
                                {
                                    return i;
                                }
                            }
                            return int.MaxValue;
                        }).ThenBy(name =>
                        {
                            var match = _regex2.Match(Path.GetFileNameWithoutExtension(name));
                            if (match.Success)
                            {
                                if (int.TryParse(match.Value, out int i))
                                {
                                    return i;
                                }
                            }
                            return int.MaxValue;
                        }).ThenBy(name => name);
        }
    }
}
