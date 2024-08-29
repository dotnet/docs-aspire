---
title: Use the .NET Aspire dashboard with Python apps
description: How to use the Aspire Dashboard in a Python application.
ms.date: 08/27/2024
ms.topic: tutorial
ms.author: aapowell
---

# Tutorial: Use the .NET Aspire dashboard with Python apps

The [.NET Aspire dashboard](overview.md) provides a great user experience for viewing telemetry, and is available as a standalone container image that can be used with any OpenTelemetry-enabled app. In this article, you'll learn how to:

> [!div class="checklist"]
>
> - Start the .NET Aspire dashboard in standalone mode.
> - Use the .NET Aspire dashboard with a Python app.

## Prerequisites

To complete this tutorial, you need the following:

- [Docker](https://docs.docker.com/get-docker/) or [Podman](https://podman.io/).
  - You can use an alternative container runtime, but the commands in this article are for Docker.
- [Python 3.9 or higher](https://www.python.org/downloads/) locally installed.
- A sample application.

## Sample application

This tutorial can be completed using either Flask, Django, or FastAPI. A sample application in each framework is provided to help you follow along with this tutorial. Download or clone the sample application to your local workstation.

### [Flask](#tab/flask)

```console
git clone https://github.com/Azure-Samples/msdocs-python-flask-webapp-quickstart
```

### [FastAPI](#tab/fastapi)

```console
git clone https://github.com/Azure-Samples/msdocs-python-fastapi-webapp-quickstart.git
```

---

To run the application locally:

### [Flask](#tab/flask)

1. Go to the application folder:

    ```console
    cd msdocs-python-flask-webapp-quickstart
    ```

1. Create a virtual environment for the app:

    [!INCLUDE [Virtual environment setup](./includes/standalone/virtual-environment-setup.md)]

1. Install the dependencies:

    ```console
    pip install -r requirements.txt
    ```

1. Run the app:

    ```console
    flask run
    ```

1. Browse to the sample application at `http://localhost:5000` in a web browser.

    :::image type="content" source="./media/standalone/run-flask-app-localhost.png" alt-text="Screenshot of the Flask app running locally in a browser":::

### [FastAPI](#tab/fastapi)

1. Go to the application folder:

    ```console
    cd msdocs-python-fastapi-webapp-quickstart
    ```

1. Create a virtual environment for the app:

    [!INCLUDE [Virtual environment setup](./includes/standalone/virtual-environment-setup.md)]

1. Install the dependencies:

    ```console
    pip install -r requirements.txt
    ```

1. Run the app:

    ```console
    uvicorn main:app --reload
    ```

1. Browse to the sample application at `http://localhost:8000` in a web browser.

    :::image type="content" source="./media/standalone/run-fastapi-app-localhost.png" alt-text="Screenshot of the FastAPI app running locally in a browser.":::

---

## Adding OpenTelemetry

To use the .NET Aspire dashboard with your Python app, you need to install the OpenTelemetry SDK and exporter. The OpenTelemetry SDK provides the API for instrumenting your application, and the exporter sends telemetry data to the .NET Aspire dashboard.

1. Install the OpenTelemetry SDK and exporter:

    ```console
    pip install opentelemetry-api opentelemetry-sdk opentelemetry-exporter-otlp-proto-grpc
    ```

1. Add a new file to your application called `otlp_tracing.py` and add the following code:

    ```python
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

### Framework Specific Instrumentation

This instrumentation has only focused on adding OpenTelemetry to our code. For more detailed instrumentation, you can use the OpenTelemetry Instrumentation packages for the specific frameworks that you are using.

### [Flask](#tab/flask)

1. Install the Flask instrumentation package:

    ```console
    pip install opentelemetry-instrumentation-flask
    ```

1. Add the following code to your application:

    ```python
    from opentelemetry.instrumentation.flask import FlaskInstrumentor

    # add this line after configure_otel_otlp() call
    FlaskInstrumentor().instrument()
    ```

### [FastAPI](#tab/fastapi)

1. Install the FastAPI instrumentation package:

    ```console
    pip install opentelemetry-instrumentation-fastapi
    ```

1. Add the following code to your application:

    ```python
    from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor

    # add this line after configure_otel_otlp() call
    FastAPIInstrumentor.instrument_app(app)
    ```

---

## Start the Aspire dashboard

To start the Aspire dashboard in standalone mode, run the following Docker command:

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:8.0.0
```

In the Docker logs, the endpoint and key for the dashboard are displayed. Copy the key and navigate to `http://localhost:18888` in a web browser. Enter the key to log in to the dashboard.

## View Structured Logs

Navigate around the Python application, and you'll see structured logs in the Aspire dashboard. The structured logs page displays logs from your application, and you can filter and search the logs.

:::image type="content" source="./media/standalone/aspire-dashboard-python-logs.png" alt-text="Screenshot of the Aspire dashboard showing the Python application logs":::

## Next steps

You have successfully used the .NET Aspire dashboard with a Python application. To learn more about the .NET Aspire dashboard, see the [Aspire dashboard overview](overview.md) and how to orchestrate a Python application with the [.NET Aspire app host](../../get-started/build-aspire-apps-with-python.md).
