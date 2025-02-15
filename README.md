# MySolution WebAPI and Vue App

This project consists of a .NET 6 WebAPI and a Vue.js frontend application. The WebAPI provides data that the Vue.js application consumes and displays in a data grid.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (version 14 or higher)
- [Vue CLI](https://cli.vuejs.org/guide/installation.html)

## Getting Started

### Clone the Repository

```sh
git clone https://github.com/yourusername/MySolution.git
cd MySolution
```

### Setting Up the WebAPI

1. Navigate to the WebAPI project directory:

    ```sh
    cd src/MySolution.WebAPI
    ```

2. Restore the .NET dependencies:

    ```sh
    dotnet restore
    ```

3. Run the WebAPI:

    ```sh
    dotnet run
    ```

   The WebAPI should now be running on `https://localhost:5014`.

### Setting Up the Vue.js Application

1. Navigate to the Vue.js project directory:

    ```sh
    cd my-vue-app
    ```

2. Install the Node.js dependencies:

    ```sh
    npm install
    ```

3. Run the Vue.js application:

    ```sh
    npm run serve
    ```

   The Vue.js application should now be running on `http://localhost:8080`.

### Accessing the Application

Open your browser and navigate to `http://localhost:8080`. You should see the Vue.js application displaying data fetched from the WebAPI.

## Troubleshooting

### CORS Issues

If you encounter CORS issues, ensure that the CORS policy is correctly configured in the WebAPI. The CORS policy should allow requests from `http://localhost:8080`.

### Network Errors

If you encounter network errors, ensure that both the WebAPI and the Vue.js application are running and accessible at their respective URLs.

## Additional Information

- The WebAPI is configured to use Swagger for API documentation. You can access it at `https://localhost:5014/swagger`.
- The Vue.js application uses Axios for making HTTP requests to the WebAPI.

## License

This project is licensed under the MIT License.