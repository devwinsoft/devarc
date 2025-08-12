using System;
using System.IO;

namespace Devarc
{
    public static class Utils
    {
        public static void MakeFolder(string dirName)
        {
            var list = dirName.Split(new char[]{'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var obj in list)
            {
                if (Directory.Exists(obj))
                {
                }
                Console.WriteLine(obj);
            }
        }
        
        // 폴더명/경로를 받아 디렉토리를 보장 생성
        public static DirectoryInfo EnsureDirectory(string pathOrName, string? baseDir = null)
        {
            // 1) ~ 홈 경로 확장
            var expanded = ExpandHome(pathOrName);

            // 2) 상대경로면 기준 디렉토리 붙임 (기본: 현재 작업 디렉토리)
            if (!Path.IsPathRooted(expanded))
                expanded = Path.Combine(baseDir ?? Directory.GetCurrentDirectory(), expanded);

            // 3) 정규화 후 생성 (하위 폴더까지 모두 생성, 이미 있어도 OK)
            var fullPath = Path.GetFullPath(expanded);
            return Directory.CreateDirectory(fullPath);
        }

        // "~", "~/" 같은 입력을 사용자 홈으로 치환
        private static string ExpandHome(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            char ds = Path.DirectorySeparatorChar;
            char ads = Path.AltDirectorySeparatorChar;

            if (path == "~") return home;
            if (path.Length >= 2 && path[0] == '~' && (path[1] == ds || path[1] == ads))
                return Path.Combine(home, path.Substring(2));

            return path;
        }
    }
}