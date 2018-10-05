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

function ResponseHandler(response, badReqMes = null)
{
    if(response.status === 200 || response.status === 201 || response.status === 204)
    {
        return response.json();
    }
    else
    {
        CreateException(response.status, badReqMes);
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
        let path = API_URL + `account/register?email=${encodeURIComponent(email)}&password=${encodeURIComponent(password)}`;
        
        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response, 'User exists');
    }

    async login(email, password)
    {
        let method = `POST`;
        let path = API_URL + `account/login?email=${encodeURIComponent(email)}&password=${encodeURIComponent(password)}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response, 'Invalid login or password');
    }

    async changePassword(oldPassword, newPassword)
    {
        let method = `PUT`;
        let path = API_URL + `account/changepassword?oldPassword=${encodeURIComponent(oldPassword)}&newPassword=${encodeURIComponent(newPassword)}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response, 'Can not used password twice or your old password is incorrect');
    }

    async changeEmail(email)
    {
        let method = `PUT`;
        let path = API_URL + `account/changeemail?email=${encodeURIComponent(email)}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response);
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

        return ResponseHandler(response);
    }

    async downloadFile(id)
    {
        return API_URL + `user/downloadfileanonymous?id=${encodeURIComponent(id)}&token=${encodeURIComponent(this.headers.Authorization)}`;
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
            return response;
        }
        else
        {
            CreateException(response.status, 'Can not generate this file.');
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

        return ResponseHandler(response);
    }

    async getBlankTypes()
    {
        let method = `GET`;
        let path = API_URL + `user/getblanktypes`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response);
    }

    async getUsers()
    {
        let method = `GET`;
        let path = API_URL + `user/getusers`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response);
    }

    async removeFile(id)
    {
        let method = `DELETE`;
        let path = API_URL + `user/removefile?id=${encodeURIComponent(id)}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response);
    }

    async addBlankType(param)
    {
        let method = `POST`;
        let path = API_URL + `user/addblanktype`;
        let body = JSON.stringify(param);
        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers,
                body: body
            }
        );

        return ResponseHandler(response, 'This type exists.');
    }

    async acceptFile(fileId)
    {
        let method = `POST`;
        let path = API_URL + `user/acceptfile?fileId=${fileId}`;

        let response = await fetch(
            path, 
            {
                method: method,
                headers: this.headers
            }
        );

        return ResponseHandler(response);
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

        return ResponseHandler(response);
    }
}