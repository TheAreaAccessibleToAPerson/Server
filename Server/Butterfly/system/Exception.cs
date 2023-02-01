using System;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.system
{
    public struct Ex
    {
        public const string x1000000 = "Вы вызвали Exception c сообщением: \n    {1} \nи передали недостаточное количесво информации для того что бы можно правильно фарматировать строку.";
        public const string x1000001 = "";
        public const string x1000002 = "";
        public const string x1000003 = "";
        public const string x1000004 = "";
        public const string x1000005 = "";
        public const string x1000006 = "";
        public const string x1000007 = "";
        public const string x1000008 = "";
        public const string x1000009 = "";
        public const string x1000010 = "";
        public const string x1000011 = "";
        public const string x1000012 = "";
        public const string x1000013 = "";
        public const string x1000014 = "";
        public const string x1000015 = "";
        public const string x1000016 = "";
        public const string x1000017 = "";
        public const string x1000018 = "";
        public const string x1000019 = "";
        public const string x1000020 = "";

        public struct PRG
        {
            public const string x1000000 = "Программа уже запущена, убедитесь что вы не создали два класса унаследованых от абстрактного класса ProgramObject.";
            public const string x1000001 = "";
            public const string x1000002 = "";
            public const string x1000003 = "";
            public const string x1000004 = "";
            public const string x1000005 = "";
            public const string x1000006 = "";
        }

        public struct STRM
        {
            public const string x100000 = "Неможет быть добавлен как выходной поток, несовподение типов.";
            public const string x100001 = "Неможет быть добавлен как входной поток, несовподение типов.";
            public const string x100002 = "Переданый тип данных в ToInput не соответсвует принимаемому типу.";
            public const string x100003 = "";
            public const string x100004 = "";
            public const string x100005 = "";
            public const string x100006 = "";
            public const string x100007 = "";
            public const string x100008 = "";
            public const string x100009 = "";
            public const string x100010 = "";
        }

        public struct MOBJ
        {
            public const string x100000 = "Вы пытаетесь повторо определить поля Program или Parent или ObjectKey.";
            public const string x100001 = "Ошибка при создании зависимостей.";
            public const string x100002 = "Вы пытаетесь добавить обьект {0} в методе AddObject() который не является типом Controller.";
            public const string x100003 = "Вы пытаетесь добавить Controller обьект {0} в метода AddObject<Type>() обьект который не унаследован от абстрактного класса MainObject.";
            public const string x100004 = "Вы патаетесь получить несущесвующий обьект по ключю с помощью метода GetObject().";
            public const string x100005 = "Не удалось добавить хендлер.";
            public const string x100006 = "Добавленый Handler не унаследован от абстрактного обьекта MainObject.";
            public const string x100007 = "Добавленый обьект не является Handler обьектом.";
            public const string x100008 = "Вы пытаетесь добавить Handler по ключу который уже используется.";
            public const string x100009 = "Вы пытаетесь добавить Controller обьект по ключу который уже используется.";
            public const string x100010 = "Вы пытаетесь удалить ребенка, но его не сущесвует.";
            public const string x100011 = "Создовать Handler обьекты можно только в методе Contruction().";
            public const string x100012 = "Вы пытаетесь задать LocalBuffer обьекту, который не реализиует интерфейс ILocalBuffer.";
            public const string x100013 = "Вы пытаетесь задать LocalBuffers обьекту, который не реализиует интерфейс ILocalBuffer.";
            public const string x100014 = "Вы пытаетесь отправить сообщение несущесвующему родителю.";
            public const string x100015 = "Вам пришоло сообщение от вашего ребенка, но вы не переопределили метод ChildrenEchoMessage.";
            public const string x100017 = "Вы пытаетесь отправить буфер вашему родителю, но у него не определен Echo<BufferType>.Children";
            public const string x100018 = "Вы пытаетесь отправить буфер вашему ребенку, но у него не определен Echo<BufferType>.Parent";
            public const string x100019 = "Вы пытаетесь добавить Hellper который не реализует интерфейс IHellper.";
            public const string x100020 = "Неудалось добавить приватный хандлер.";
            public const string x100021 = "Вы пытаетесь отправить сообщение ребенку которого не сущесвует.";
            public const string x100022 = "Вы не можете оправить сообщение своему родителю, так как этот обьект первородный.";
            public const string x100023 = "Вы пытаетесь отправить буфер своему предку, но у него не определен EchoChildren<BufferType>.";
            public const string x100024 = "Вы не можете отправить буфер вашему родителю_, так как ваш родитель это первородный обьект.";
            public const string x100025 = "Вы не можете отправить сообщение вашему предку, так как вышли за границу древа.";
            public const string x100026 = "Вы не смогли оправить сообщение своему предку по его имени, так как родителя с таким именем нету.";
            public const string x100027 = "Приватные Handler'ы могут быть добавлены только во время создания обьекта, с помощью зависимостей.";
            public const string x100028 = "Запустить сборщик мусора можно только из ProgramController обьекта.";
            public const string x100029 = "Обьек должен находится в пространсве имен Butterfly.";
            public const string x100030 = "Handler обьекта с таким ключом не сущесвует.";
            public const string x100031 = "Handler обьекта с таким нипом не сущесвует.";
            public const string x100032 = "Не удалось добавить приватный Handler.";
            public const string x100033 = "Добавить дочерний Controller обьект {0} можно только в методе Start() или во время выполнения программы.";
            public const string x100034 = "Вы не можете удалить обьект с ключом {0} потому что его не сущесвует.";
            public const string x100035 = "Вы пытаетесь добавить обьект Hellper, он но не унаследован от интерфеса IHellper.";
            public const string x100036 = "Вы не можете отправить сообщение родительскому слушателю из обьекта ProgramController.";
            public const string x100037 = "Нету неодного родительского обьекта который бы прослушивал сообщение {0} от {1}.";
            public const string x100039 = "Вы не можете отправить буфер в родительский обьект так как у вас его нету.";
            public const string x100040 = "В родительском обьекте не определен Handler обьект {0} который бы смог принять от вас буффер.";
            public const string x100041 = "Вы неможете отправить буфер в дочерний обьект, так как там не определен {0}.";
            public const string x100042 = "Вы не можете отпровлять буферы обьектам в методе Construction().";
            public const string x100043 = "";
            public const string x100044 = "";
            public const string x100045 = "";
            public const string x100046 = "";
            public const string x100047 = "";
        }

        public struct THRD
        {
            public const string x400000 = "Было указано пустое имя потока.";
            public const string x400001 = "При определение свойств потока, необходимо сначало задать свойсво для Name.";
            public const string x400002 = "При определение свойств потока, свойство для ThreadCount задается после свойсва ThreadTimeDelay.";
            public const string x400003 = "Невозможно определить свойсва для ThreadTimeDelay и ThreadCount меньше или равное нулю.";
            public const string x400004 = "Вы не переопределил метод UpdateThread().";
            public const string x400005 = "Создать локеры можно только в конструкторе или в методе Start().";
            public const string x400006 = "Вы пытаетесь получить несущесвующий Locker.";
            public const string x400007 = "Locker с таким именем уже сущесвует.";
            public const string x400008 = "Неверно определны свойсва для ThreadTimeDelay или ThreadCount или Name.";
            public const string x400009 = "Вы повторно задали MainAction в методе SetMainAction().";
            public const string x400010 = "Вызвать фуркций AddThread() можно только в переопределеном методе Start().";
        }

        public struct HNDR
        {
            public const string x100000 = "Обьек должен находится в пространсве имен Butterfly.";
            public const string x100001 = "Handler обьекта с таким типом не сущесвует.";
            public const string x100002 = "Handler обьекта c таким ключом не сущесвует.";
            public const string x100003 = "Переданый обьект в метод redirect_to<StreamType>() не реализует интерфейс IStream.";
            public const string x100004 = "Вы не можете отправить сообщение родительскому обьекту из Action.";
            public const string x100005 = "Указать метод send_to_parent без явного укзания типа можно только из Handler'a.";
            public const string x100006 = "Вы неможете указать метод send_to_parent к прикрепляемому Handler'у. Так как это лишь ссылка на него.";
            public const string x100007 = "Назначить InputStream с помощью метода SetInputStream() можно только в методе Construction().";
            public const string x100008 = "Переназначить InputSteam c помощью метода SetInputStream() можно лишь один раз.";
            public const string x100009 = "Метод redirect_to(Action<Type>) можно вызвать лишь однажды и самым первым, перед тем как указать остальные редиректы.";
            public const string x100010 = "Переназначить InputStream с помощью метода SetInputStream() в Handler обьекте с типом EchoHandler, нельзя.";
            public const string x100011 = "Вы попытались задать Handler'y со Stream типом INPUT_STREAM выходной поток. Handler с таким типом лишь принимает буферы и неимеет выходных потоков.";
            public const string x100012 = "В Handler'e с Stream типом INPUT_STREAM нужно явно задать задать InputStream с помощью метода SetInputStream().";
            public const string x100013 = "Метод lock_redirect_to(Action<Type>) можно вызвать лишь однажды и самым первым, перед тем как указать остальные редиректы.";
            public const string x100014 = "Вы не можете отправить сообщение дочернему обьекту из Action.";
            public const string x100015 = "Уберите явное указание типа для прикрепляемого Action в первом redirect_to.";
            public const string x100016 = "C помощью метода .redirect_to<StreamType>() можно перенаправить поток только в анализатор или в приватный обработчик.";
            public const string x100017 = "Вы передали в .redirect_to<{0}>(object LocalBuffer) анализатор который не реализует интерфейс ILocalBuffer.";
            public const string x100019 = "";
            public const string x100020 = "";
        }

        public struct LB
        {
            public const string x100000 = "Вы присвоили LocalBuffer неверного типа.";
            public const string x100001 = "Вы присвоили LocalBuffers неверного типа.";
        }

        public struct AN
        {
            public const string x100000 = "Вы пытаетесь перенаправить выходной поток несущесвующий обьект.";
            public const string x100001 = "";
            public const string x100002 = "";
            public const string x100003 = "";
            public const string x100004 = "";
            public const string x100005 = "";
            public const string x100006 = "";
        }
    }
}
