////Laba3

//using HtmlAgilityPack;
//using System;
//using System.Linq;

//class Program
//{
//    static void Main(string[] args)
//    {
//        var url = "https://www.python.org/";
//        var web = new HtmlWeb();
//        var doc = web.Load(url);

//        // Задание 1: Найти изображение img в заголовке h1 с использованием XPath и вывести ссылку на изображение.
//        var imgInH1 = doc.DocumentNode.SelectSingleNode("//h1//img");
//        if (imgInH1 != null)
//        {
//            var imgSrc = imgInH1.GetAttributeValue("src", "");
//            Console.WriteLine($"Заголовок страницы: {imgSrc}");
//        }

//        // Задание 2: Найти и вывести ссылки всех элементов a в разделе "About" с помощью XPath.
//        var aboutLinks = doc.DocumentNode.SelectNodes("//li[@id='about']//a");
//        if (aboutLinks != null)
//        {
//            Console.WriteLine("\nСсылки в разделе 'About':");
//            foreach (var link in aboutLinks)
//            {
//                var href = link.GetAttributeValue("href", "");
//                Console.WriteLine(href);
//            }
//        }

//        // Задание 3: Найти и вывести текст всех заголовков h2 с помощью CSS-селектора.
//        var h2Headers = doc.DocumentNode.SelectNodes("//h2");
//        if (h2Headers != null)
//        {
//            Console.WriteLine("\nЗаголовки h2 на странице:");
//            foreach (var header in h2Headers)
//            {
//                Console.WriteLine(header.InnerText.Trim());
//            }
//        }

//        // Задание 4: Найти и вывести ссылки всех элементов a в родительском контейнере Navigation Menu с помощью CSS-селектора.
//        var navMenuLinks = doc.DocumentNode.SelectNodes("//ul[@id='mainnav']//a");
//        if (navMenuLinks != null)
//        {
//            Console.WriteLine("\nНавигационные ссылки:");
//            foreach (var link in navMenuLinks)
//            {
//                var href = link.GetAttributeValue("href", "");
//                Console.WriteLine(href);
//            }
//        }
//    }
//}


//Laba4
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Получаем путь к рабочему столу пользователя
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Инициализация веб-драйвера Chrome
        IWebDriver driver = new ChromeDriver();

        // URL страницы VK Video
        var url = "https://vk.com/video";
        driver.Navigate().GoToUrl(url);

        // Ожидание, чтобы страница полностью загрузилась
        Thread.Sleep(5000);

        List<string> videoLinks = new List<string>();

        try
        {
            // Получение всех видео из разделов "Для вас" и "Тренды"
            var videoElements = driver.FindElements(By.XPath("//a[contains(@href, '/video')]"));

            foreach (var video in videoElements)
            {
                string videoHref = video.GetAttribute("href");
                videoLinks.Add(videoHref);
            }

            // Запись количества ссылок на видео в файл на рабочем столе
            File.WriteAllText(Path.Combine(desktopPath, "video_links.txt"), videoLinks.Count.ToString());

            // Открытие каждого видео для сбора информации
            using (StreamWriter file = new StreamWriter(Path.Combine(desktopPath, "video_info.txt")))
            {
                int processedVideos = 0; // Счетчик обработанных видео

                foreach (string videoLink in videoLinks)
                {
                    if (processedVideos >= 5) // Ограничение на 5 видео
                    {
                        break;
                    }

                    try
                    {
                        driver.Navigate().GoToUrl(videoLink);
                        Thread.Sleep(3000);  // ожидание загрузки страницы видео

                        // Сбор информации о видео
                        string title = driver.FindElement(By.XPath("//div[contains(@class, 'VideoCard__title')]")).Text;
                        string channelName = driver.FindElement(By.XPath("//a[contains(@class, 'VideoCard__ownerLink')]")).Text;
                        string views = driver.FindElement(By.XPath("//span[contains(@class, 'VideoCard__extendedInfoView')]")).Text;
                        string date = driver.FindElement(By.XPath("//span[contains(@class, 'VideoCard__extendedInfoUpdated')]")).Text;

                        // Запись информации о видео в файл
                        file.WriteLine($"Название: {title}, Канал: {channelName}, Просмотров: {views}, Дата: {date}");

                        processedVideos++; // Увеличиваем счетчик обработанных видео
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке видео {videoLink}: {ex.Message}");
                        continue;
                    }
                }
            }
        }
        finally
        {
            // Закрытие браузера
            driver.Quit();
        }
    }
}
