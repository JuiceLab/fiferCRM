var ru2en = {
    ru_str: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя №\"()%./-:«»0123456789",
    en_str: ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'V', 'G', 'D', 'E', 'JO', 'ZH', 'Z', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T',
    'U', 'F', 'H', 'C', 'CH', 'SH', 'SHH', '', 'I', '', 'JE', 'JU',
    'JA', 'a', 'b', 'v', 'g', 'd', 'e', 'jo', 'zh', 'z', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'f',
    'h', 'c', 'ch', 'sh', 'shh', '', 'i', '', 'je', 'ju', 'ja', '_', '_', '', '', '', '', '', '', '', '_', '_', '_', '_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'],
    translit: function (org_str) {
        var tmp_str = "";
        for (var i = 0, l = org_str.length; i < l; i++) {
            var s = org_str.charAt(i), n = this.ru_str.indexOf(s);
            if (n >= 0) { tmp_str += this.en_str[n]; }
            else { tmp_str += '_'; }
        }
        return tmp_str;
    }
}
function translit(source, dest) {
    $(dest).val(ru2en.translit($(source).val()).toLowerCase());
}