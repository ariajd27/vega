window.cookies = {
    set: function (name, value)
    {
        const expirationDate = new Date();
        expirationDate.setTime(expirationDate.getTime() + (30 * 24 * 60 * 60 * 1000));
        let expires = "; expires=" + expirationDate.toUTCString();

        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    },

    get: function (name)
    {
        const nameEQ = name + "=";
        const ca = document.cookie.split(';');

        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
        }

        return null;
    },

    delete: function (name)
    {
        document.cookie = name + '=; Max-Age=-9999;';
    }
};