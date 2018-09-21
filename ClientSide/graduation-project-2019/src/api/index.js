const API_URL = `https://graduationprojectapi20180920024232.azurewebsites.net/api/`;

class StatusException
{
    constructor(status, message)
    {
        this.status = status;
        this.message = message;
    }
}

function CreateException(status, messageBadRequst = null)
{
    if(status === 400)
    {
        throw new StatusException(status, messageBadRequst);
    }
    if(status === 401)
    {
        throw new StatusException(status, 'Not logined');
    }
    if(status === 403)
    {
        throw new StatusException(status, 'Unauthorized access');
    }
    if(status === 404)
    {
        throw new StatusException(status, 'Not found');
    }
    if(status === 422)
    {
        throw new StatusException(status, 'Invalid params');
    }
    if(status === 500)
    {
        throw new StatusException(status, 'Server exception');
    }
}

export default class ApiRequests
{
    asyncStorageUser = 'gp-2019-user';

    headers = {
        Accept: 'application/json',
        'Content-Type': 'application/json'
    };

    setAuthorizationHeader(value)
    {
        this.headers = {
            Accept: 'application/json',
            'Content-Type': 'application/json',
            Authorization: value
        }
    }

    async register(email, password)
    {
        let method = `POST`;
        let path = API_URL + `account/register?email=${email}&password=${password}`;
        
        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status, 'User exists');
        }

        return response.json();
    }

    async login(email, password)
    {
        let method = `POST`;
        let path = API_URL + `account/login?email=${email}&password=${password}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status, 'Invalid login or password');
        }
    }

    async changePassword(oldPassword, newPassword)
    {
        let method = `PUT`;
        let path = API_URL + `account/changepassword?oldPassword=${oldPassword}&newPassword=${newPassword}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status, 'Can not used password twice or your old password is incorrect');
        }
    }

    async changeEmail(email)
    {
        let method = `PUT`;
        let path = API_URL + `account/changeemail?email=${email}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status);
        }
    }

    async updateToken()
    {
        let method = `POST`;
        let path = API_URL + `account/updatetoken`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status);
        }
    }

    async downloadFile(id)
    {
        let method = `GET`;
        let path = API_URL + `user/downloadfile?id=${id}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status);
        }
    }

    async generateExcel(param)
    {
        let method = `POST`;
        let path = API_URL + `user/generateexcel`;
        let body = JSON.stringify(param);

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers,
                body: body
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status, 'Can not generate this file. Your file is wrong or file with this name exists');
        }
    }

    async getFiles()
    {
        let method = `GET`;
        let path = API_URL + `user/getfiles`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status);
        }
    }

    async removeFile(id)
    {
        let method = `DELETE`;
        let path = API_URL + `user/removefile?id=${id}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status);
        }
    }

    async sendMessage(mes)
    {
        let method = `POST`;
        let path = API_URL + `user/sendmessage`;
        let body = JSON.stringify(mes);

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers,
                body: body
            }
        );

        if(response.status === 200 || response.status === 201 || response.status === 204)
        {
            return response.json();
        }
        else
        {
            CreateException(response.status);
        }
    }
}