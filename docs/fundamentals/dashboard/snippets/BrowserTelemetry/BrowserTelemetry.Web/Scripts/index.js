import { ConsoleSpanExporter, SimpleSpanProcessor } from '@opentelemetry/sdk-trace-base';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-proto';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { Resource } from '@opentelemetry/resources';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';

export function initializeTelemetry(otlpUrl, headers, resourceAttributes) {
    const otlpOptions = {
        url: `${otlpUrl}/v1/traces`,
        headers: parseDelimitedValues(headers)
    };

    const attributes = parseDelimitedValues(resourceAttributes);
    attributes[SemanticResourceAttributes.SERVICE_NAME] = 'browser';

    const provider = new WebTracerProvider({
        resource: new Resource(attributes),
    });
    provider.addSpanProcessor(new SimpleSpanProcessor(new ConsoleSpanExporter()));
    provider.addSpanProcessor(new SimpleSpanProcessor(new OTLPTraceExporter(otlpOptions)));

    provider.register({
        // Prefer ZoneContextManager: supports asynchronous operations
        contextManager: new ZoneContextManager(),
    });

    // Registering instrumentations
    registerInstrumentations({
        instrumentations: [new DocumentLoadInstrumentation()],
    });
}

function parseDelimitedValues(s) {
    const headers = s.split(','); // Split by comma
    const result = {};

    headers.forEach(header => {
        const [key, value] = header.split('='); // Split by equal sign
        result[key.trim()] = value.trim(); // Add to the object, trimming spaces
    });

    return result;
}
