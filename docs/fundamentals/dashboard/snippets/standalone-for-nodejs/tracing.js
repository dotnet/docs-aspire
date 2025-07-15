const { NodeSDK } = require('@opentelemetry/sdk-node');
const { getNodeAutoInstrumentations } = require('@opentelemetry/auto-instrumentations-node');
const { OTLPTraceExporter } = require('@opentelemetry/exporter-otlp-grpc');
const { OTLPMetricExporter } = require('@opentelemetry/exporter-otlp-grpc');
const { OTLPLogExporter } = require('@opentelemetry/exporter-otlp-grpc');
const { PeriodicExportingMetricReader } = require('@opentelemetry/sdk-metrics');

// Configure the OTLP endpoint - this should match your dashboard configuration
const otlpEndpoint = process.env.OTEL_EXPORTER_OTLP_ENDPOINT || 'http://localhost:4317';

const sdk = new NodeSDK({
  traceExporter: new OTLPTraceExporter({
    url: otlpEndpoint,
  }),
  metricReader: new PeriodicExportingMetricReader({
    exporter: new OTLPMetricExporter({
      url: otlpEndpoint,
    }),
    exportIntervalMillis: 5000,
  }),
  logRecordProcessor: {
    exporter: new OTLPLogExporter({
      url: otlpEndpoint,
    }),
  },
  instrumentations: [getNodeAutoInstrumentations({
    '@opentelemetry/instrumentation-fs': {
      enabled: false,
    },
  })],
});

sdk.start();

console.log('OpenTelemetry started successfully');

process.on('SIGTERM', () => {
  sdk.shutdown()
    .then(() => console.log('Tracing terminated'))
    .catch((error) => console.log('Error terminating tracing', error))
    .finally(() => process.exit(0));
});