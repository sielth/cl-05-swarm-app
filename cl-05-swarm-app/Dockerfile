FROM mcr.microsoft.com/dotnet/aspnet:7.0

COPY bin/Debug /app

RUN echo '#!/bin/sh' > startup.sh && \
    echo 'cd app/net7.0' >> startup.sh && \
    echo 'dotnet cl-05-swarm-app.dll' >> startup.sh && \
    chmod +x startup.sh 

ENTRYPOINT [ "sh", "startup.sh" ]