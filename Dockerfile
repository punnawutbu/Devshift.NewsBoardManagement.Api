FROM mcr.microsoft.com/dotnet/aspnet:6.0.27

# Set time zone
ENV TZ=Asia/Bangkok
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app

COPY ["./publish", "."]

ENTRYPOINT ["sh", "-c", "update-ca-certificates && rm -f core.* && dotnet Devshift.NewsBoardManagement.Api.dll"]

EXPOSE 32002