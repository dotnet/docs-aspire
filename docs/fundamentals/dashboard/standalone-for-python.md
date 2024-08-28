---
title: Using Aspire Dashboard with Python Applications
description: How to use the Aspire Dashboard in a Python application.
ms.date: 08/27/2024
ms.topic: Tutorial
---

# Tutorial: Using the Aspire Dashboard with Python Applications

The [Aspire dashboard](overview.md) provides a great UI for viewing telemetry, and is available as a standalone container image that can be used with any OpenTelemetry-enabled app. In this article, you'll learn how to:

> [!div class="checklist"]
>
> - Start the Aspire dashboard in standalone mode.
> - Use the Aspire dashboard with a Python application.

## Prerequisites

To complete this tutorial, you need the following:

- [Docker](https://docs.docker.com/get-docker/)
  - You can use an alternative container runtime, but the commands in this article are for Docker.
- [Python 3.12 or higher](https://www.python.org/downloads/) locally installed
- A sample allication

## Sample application

This quickstart can be completed using either Flask, Django, or FastAPI. A sample application in each framework is provided to help you follow along with this quickstart. Download or clone the sample application to your local workstation.

### [Flask](#tab/flask)

```Console
git clone https://github.com/Azure-Samples/msdocs-python-flask-webapp-quickstart
```

### [FastAPI](#tab/fastapi)

```Console
git clone https://github.com/Azure-Samples/msdocs-python-fastapi-webapp-quickstart.git
```

---

To run the application locally:

### [Flask](#tab/flask)

1. Go to the application folder:

    ```Console
    cd msdocs-python-flask-webapp-quickstart
    ```

1. Create a virtual environment for the app:

    [!INCLUDE [Virtual environment setup](./includes/quickstart-python/virtual-environment-setup.md)]

1. Install the dependencies:

    ```Console
    pip install -r requirements.txt
    ```

1. Run the app:

    ```Console
    flask run
    ```

1. Browse to the sample application at `http://localhost:5000` in a web browser.

    :::image type="content" source="./media/run-flask-app-localhost.png" alt-text="Screenshot of the Flask app running locally in a browser":::

### [FastAPI](#tab/fastapi)

1. Go to the application folder:

    ```Console
    cd msdocs-python-fastapi-webapp-quickstart
    ```

1. Create a virtual environment for the app:

    [!INCLUDE [Virtual environment setup](./includes/virtual-environment-setup.md)]

1. Install the dependencies:

    ```Console
    pip install -r requirements.txt
    ```

1. Run the app:

    ```Console
    uvicorn main:app --reload
    ```

1. Browse to the sample application at `http://localhost:8000` in a web browser.

    :::image type="content" source="./media/run-django-app-localhost.png" alt-text="Screenshot of the FastAPI app running locally in a browser.":::

---

## Adding OpenTelemetry

To use the Aspire dashboard with your Python application, you need to install the OpenTelemetry SDK and exporter. The OpenTelemetry SDK provides the API for instrumenting your application, and the exporter sends telemetry data to the Aspire dashboard.

1. Install the OpenTelemetry SDK and exporter:

    ```Console
    pip install opentelemetry-api opentelemetry-sdk opentelemetry-exporter-otlp-proto-grpc
    ```

1. Add a new file to your application called `otlp_tracing.py` and add the following code:

    ```Python
    import logging
    from opentelemetry import metrics, trace

    from opentelemetry._logs import set_logger_provider
    from opentelemetry.exporter.otlp.proto.grpc._log_exporter import (
        OTLPLogExporter,
    )
    from opentelemetry.exporter.otlp.proto.grpc.metric_exporter import OTLPMetricExporter
    from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter
    from opentelemetry.sdk._logs import LoggerProvider, LoggingHandler
    from opentelemetry.sdk._logs.export import BatchLogRecordProcessor
    from opentelemetry.sdk.metrics import MeterProvider
    from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader
    from opentelemetry.sdk.trace import TracerProvider
    from opentelemetry.sdk.trace.export import BatchSpanProcessor

    def configure_oltp_grpc_tracing(
        endpoint: str = None
    ) -> trace.Tracer:
        # Configure Tracing
        traceProvider = TracerProvider()
        processor = BatchSpanProcessor(OTLPSpanExporter(endpoint=endpoint))
        traceProvider.add_span_processor(processor)
        trace.set_tracer_provider(traceProvider)

        # Configure Metrics
        reader = PeriodicExportingMetricReader(OTLPMetricExporter(endpoint=endpoint))
        meterProvider = MeterProvider(metric_readers=[reader])
        metrics.set_meter_provider(meterProvider)

        # Configure Logging
        logger_provider = LoggerProvider()
        set_logger_provider(logger_provider)

        exporter = OTLPLogExporter(endpoint=endpoint)
        logger_provider.add_log_record_processor(BatchLogRecordProcessor(exporter))
        handler = LoggingHandler(level=logging.NOTSET, logger_provider=logger_provider)
        handler.setFormatter(logging.Formatter("Python: %(message)s"))

        # Attach OTLP handler to root logger
        logging.getLogger().addHandler(handler)

        tracer = trace.get_tracer(__name__)
        return tracer
    ```

1. Update your application (`app.py` for Flask, `main.py` for FastAPI) to include the imports and call the `configure_oltp_grpc_tracing` function:

    ```python
    import logging
    from otlp_tracing import configure_otel_otlp

    logging.basicConfig(level=logging.INFO)
    tracer = configure_otel_otlp()
    logger = logging.getLogger(__name__)
    ```

1. Replace the `print` calls with `logger.info` calls in your application.
1. Restart your application.

## Start the Aspire dashboard

To start the Aspire dashboard in standalone mode, run the following Docker command:

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:8.0.0
```

In the Docker logs, the endpoint and key for the dashboard are displayed. Copy the key and navigate to `http://localhost:18888` in a web browser. Enter the key to log in to the dashboard.

## Viewing Structured Logs

Navigate around the Python application, and you'll see structured logs in the Aspire dashboard. The structured logs page displays logs from your application, and you can filter and search the logs.

:::image type="content" source="./media/aspire-dashboard-python-logs.png" alt-text="Screenshot of the Aspire dashboard showing the Python application logs":::

## Next steps

Congratulations! You have successfully used the Aspire dashboard with a Python application. To learn more about the Aspire dashboard, see the [Aspire dashboard overview](overview.md) and how to orchestrate a Python application with the [Aspire App Host](../get-started/build-aspire-apps-with-python.md).
